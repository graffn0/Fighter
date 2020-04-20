namespace Fighter.ViewResolvers

open Fighter
open Godot
open EcsRx.Collections.Database
open EcsRx.Events
open EcsRx.Groups
open EcsRx.Entities
open EcsRx.Extensions
open EcsRx.Plugins.Views.Components
open EcsRx.Plugins.Views.Systems
open EcsRx.Plugins.Views.ViewHandlers
open Components.UI
open Components.Node

open ViewHandlers

type ViewResolver(collectionManager: IEntityDatabase, eventSystem: IEventSystem) =
    inherit ViewResolverSystem(eventSystem)

    let mutable mainHandler = Unchecked.defaultof<IViewHandler>

    member this.collectionManager = collectionManager

    override this.Group = new Group(typedefof<ViewComponent>, typedefof<TypeComponent>) :> IGroup

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
                    let scene = ResourceLoader.Load<PackedScene>(typeComponent.loadPath)
                    let instance = scene.Instance()
                    Applicationfs.instance.AddChild(instance)
                    instance )
        viewComponent.View <- handler.CreateView()
        mainHandler <- handler
        this.OnViewCreated(entity, viewComponent)

    override this.OnViewCreated(entity: IEntity, viewComponent: ViewComponent) =
        let typeComponent = entity.GetComponent<TypeComponent>()

        let addNodeComponents(view: EntityBoundfs) =
            let spritePath = new NodePath("Sprite")
            let collisionPath = new NodePath("CollisionShape2D")

            let setupSprite(view: EntityBoundfs) =
                match view.GetNode(spritePath) with
                | :? Sprite as sprite -> entity.AddComponents({ sprite = sprite })
                | _ -> ()

            let setupCollision(view: EntityBoundfs) =
                match view.GetNode(collisionPath) with
                | :? CollisionShape2D as collision -> entity.AddComponents({ collision = collision })
                | _ -> ()

            if view.HasNode(spritePath) then setupSprite(view)
            if view.HasNode(collisionPath) then setupCollision(view)

        match viewComponent.View with
        | :? EntityBoundfs as view ->
            addNodeComponents(view)
            view.entityCollection <- Some (this.collectionManager.GetCollectionFor(entity))
            view.entity <- Some (entity)
            view.Name <- typeComponent.name
        | _ -> ()