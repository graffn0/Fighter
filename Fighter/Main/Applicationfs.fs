namespace Fighter

open System.Threading
open System.Collections.Generic
open Fighter.Components
open Godot
open EcsRx
open EcsRx.Entities
open EcsRx.Infrastructure.Dependencies
open EcsRx.Extensions
open EcsRx.Godot
open EcsRx.Components.Lookups
open EcsRx.Plugins.Views.Components

type CustomComponentLookupsModule() =
    interface IDependencyModule with
        member this.Setup(container: IDependencyContainer) =
            GD.Print("test")
            container.Unbind(typedefof<IComponentTypeLookup>)
            let explicitTypeLookups = new Dictionary<System.Type, int>()
            explicitTypeLookups.Add(typedefof<ViewComponent>, 0)
            let explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups)
            container.Bind(typedefof<IComponentTypeLookup>, new BindingConfiguration(ToInstance = explicitComponentLookup))

[<AbstractClass>]
type Applicationfs() =
    inherit GodotApplication()

    static member val instance: Node = new Node() with get, set

    override this.ApplicationStarted() =
        this.Container.Unbind(typedefof<IComponentTypeLookup>)
        let explicitTypeLookups = new Dictionary<System.Type, int>()
        explicitTypeLookups.Add(typedefof<ViewComponent>, 0)
        let explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups)
        this.Container.Bind(typedefof<IComponentTypeLookup>, new BindingConfiguration(ToInstance = explicitComponentLookup))
        //thes.Setup(this.Container)
        //Applicationfs.instance <- this
        let defaultCollection = this.EntityCollectionManager.GetCollection()
        let entity = defaultCollection.CreateEntity()
        entity.AddComponent<ViewComponent>() |> ignore
        //entity.AddComponents(new MovementComponent(), new ViewComponent())

        //let playerView = entity.GetComponent(typedefof<ViewComponent>)
        //let viewGameObject = entity.GetView()

        //viewGameObject.Position <- Vector2(600.0f, 400.0f)
        GD.Print("test2")