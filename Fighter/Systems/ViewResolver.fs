namespace Fighter.ViewResolvers

open Fighter
open Godot
open EcsRx.Collections.Database
open EcsRx.Groups
open EcsRx.Entities
open EcsRx.Extensions
open EcsRx.Plugins.Views.Components
open EcsRx.Plugins.Views.Systems
open EcsRx.Plugins.Views.ViewHandlers
open SystemsRx.Events
open Components.UI
open Components.Node
open Components.Combat
open ViewHandlers

type ViewResolver(collectionManager: IEntityDatabase, eventSystem: IEventSystem) =
    inherit ViewResolverSystem(eventSystem)

    let mutable mainHandler = Unchecked.defaultof<IViewHandler>

    member this.collectionManager = collectionManager

    override this.Group =
        Group(typedefof<ViewComponent>, typedefof<TypeComponent>) :> IGroup

    override this.ViewHandler
        with get() = mainHandler

    override this.Setup(entity: IEntity) =
        let viewComponent = entity.GetComponent<ViewComponent>()
        let typeComponent = entity.GetComponent<TypeComponent>()
        let nodePath = new NodePath(typeComponent.name)
        let handler =
            viewHandler(
                if Applicationfs.instance.HasNode(nodePath) then
                    Applicationfs.instance.GetNode(nodePath)
                else
                    let scene =
                        ResourceLoader.Load<PackedScene>(typeComponent.loadPath)
                    let instance = scene.Instance()
                    Applicationfs.instance.AddChild(instance)
                    instance )
        viewComponent.View <- handler.CreateView()
        mainHandler <- handler
        this.OnViewCreated(entity, viewComponent)

    override this.OnViewCreated(entity: IEntity, viewComponent: ViewComponent) =
        let typeComponent = entity.GetComponent<TypeComponent>()

        let addNodeComponents() =
            let setupHealthBar() =
                let healthComponent = entity.GetComponent<HealthComponent>()
                let playerComponent = entity.GetComponent<PlayerComponent>()
                let container =
                    Applicationfs
                        .instance
                        .GetNode(new NodePath("CanvasLayer"))
                        .GetNode(new NodePath("Interface"))
                        .GetNode(new NodePath("HBoxContainer"))

                let addComponent(path: NodePath) =
                    match path |> container.GetNode with
                    | :? TextureProgress as node ->
                        node.MaxValue <- healthComponent.totalHealth
                        node.Value <- healthComponent.totalHealth
                        { healthBar = node }
                        |> entity.AddComponents
                    | _ -> ()

                match playerComponent.id with
                | 1 -> addComponent(new NodePath("Bar1"))
                | 2 -> addComponent(new NodePath("Bar2"))
                | _ -> ()
            
            if entity.HasComponent<HealthComponent>() &&
                entity.HasComponent<PlayerComponent>() then setupHealthBar()

        let addNodeComponents2(node: Node) =
            match node with
            | :? AnimationTree as animationTree ->
                let stateMachine = animationTree.Get("parameters/StateMachine/playback") :?> AnimationNodeStateMachinePlayback
                let stateComponent = entity.GetComponent<StateComponent>()
                stateComponent.currentState.Value
                |> stateToString
                |> stateMachine.Travel
            | _ -> ()

        match viewComponent.View with
        | :? EntityBoundfs as view ->
            addNodeComponents()
            view.entityCollection <-
                Some (this.collectionManager.GetCollectionFor(entity))
            view.entity <- Some (entity)
            view.Name <- typeComponent.name
        | _ -> ()