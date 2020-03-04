namespace Fighter

//open System.Collections.Generic
//open Godot
//open EcsRx.Infrastructure.Dependencies
//open EcsRx.Components.Lookups
//open EcsRx.Plugins.Views.Components

//type CustomComponentLookupsModule() =
    //interface IDependencyModule with
        //member this.Setup(container: IDependencyContainer) =
            //GD.Print("test")
            //container.Unbind(typedefof<IComponentTypeLookup>)
            //let explicitTypeLookups = new Dictionary<System.Type, int>()
            //explicitTypeLookups.Add(typedefof<ViewComponent>, 0)
            //let explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups)
            //container.Bind(typedefof<IComponentTypeLookup>, new BindingConfiguration(ToInstance = explicitComponentLookup))