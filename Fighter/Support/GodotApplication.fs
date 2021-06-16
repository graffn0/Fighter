namespace Fighter

open System.Linq
open Godot
open EcsRx.Collections
open EcsRx.Collections.Database
open EcsRx.Infrastructure
open EcsRx.Infrastructure.Modules
open EcsRx.Infrastructure.Ninject
open EcsRx.Plugins.ReactiveSystems
open EcsRx.Plugins.Views
open SystemsRx.Executor
open SystemsRx.Events
open SystemsRx.Infrastructure.Dependencies
open SystemsRx.Infrastructure.Extensions;
open SystemsRx.Infrastructure.Modules
open SystemsRx.Infrastructure.Plugins
open SystemsRx.Systems

[<AbstractClass>]
type GodotApplication() = 
    inherit Node()

    let container = new NinjectDependencyContainer()
    let mutable systemExecutor = Unchecked.defaultof<ISystemExecutor>
    let mutable eventSystem = Unchecked.defaultof<IEventSystem>
    let mutable entityDatabase = Unchecked.defaultof<IEntityDatabase>
    let mutable observableGroupManager = Unchecked.defaultof<IObservableGroupManager>
    let mutable plugins = Seq.empty

    member this.Application = 
        { new IEcsRxApplication with
        member this.Container = container :> IDependencyContainer
        member this.SystemExecutor 
            with get() = systemExecutor
        member this.EventSystem
            with get() = eventSystem
        member this.EntityDatabase
            with get() = entityDatabase
        member this.ObservableGroupManager
            with get() = observableGroupManager
        member this.Plugins
            with get() = plugins

        member this.StartApplication() =
            let registerPlugin(plugin: ISystemsRxPlugin) =
                plugins <- plugins.Append(plugin)

            let loadPlugins() =
                registerPlugin(new ViewsPlugin())
                registerPlugin(new ReactiveSystemsPlugin())

            let loadModules() =
                this.Container.LoadModule<EcsRxInfrastructureModule>()
                this.Container.LoadModule<FrameworkModule>()

            let resolveApplicationDependencies() =
                systemExecutor <- this.Container.Resolve<ISystemExecutor>()
                eventSystem <- this.Container.Resolve<IEventSystem>()
                entityDatabase <- this.Container.Resolve<IEntityDatabase>()
                observableGroupManager <- this.Container.Resolve<IObservableGroupManager>()

            let bindSystems() =
                let mainNamespace =
                    this.GetType()
                        .Namespace
                        .Replace("<StartupCode$", "")
                        .Replace(">", "")
                let namespaces = [|
                    mainNamespace + ".Systems"
                    mainNamespace + ".ViewResolvers"
                |]
                this.BindAllSystemsInNamespaces(namespaces)

            let startSystems() =
                this.StartAllBoundSystems()

            let setupPlugins() =
                for x in this.Plugins do 
                    x.SetupDependencies(this.Container)

            let startPluginSystems() =
                let systems = 
                    this.Plugins.SelectMany(fun x -> 
                        x.GetSystemsForRegistration(this.Container))
                for x in systems do
                    this.SystemExecutor.AddSystem(x)

            loadModules()
            loadPlugins()
            setupPlugins()
            resolveApplicationDependencies()
            bindSystems()
            startPluginSystems()
            startSystems()

        member this.StopApplication() =
            let stopAndUnbindAllSystems() =
                let allSystems = this.SystemExecutor.Systems.ToList()
                allSystems.ForEach(fun x -> this.SystemExecutor.RemoveSystem(x))
                this.Container.Unbind<ISystem>()
            stopAndUnbindAllSystems() }

    abstract member ApplicationStarted: unit -> unit

    override this._EnterTree() =
        this.Application.StartApplication()
        this.ApplicationStarted()

    override this._ExitTree() =
        this.Application.StopApplication()