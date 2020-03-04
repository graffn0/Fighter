namespace Fighter.ViewResolvers

//open Fighter
//open Fighter.Handlers
//open Fighter.Components
//open Godot
//open EcsRx.Collections
//open EcsRx.Events
//open EcsRx.Groups
//open EcsRx.Entities
//open EcsRx.Extensions
//open EcsRx.Plugins.Views.Components
//open EcsRx.Plugins.Views.Systems
//open EcsRx.Plugins.Views.ViewHandlers

//type PlayerViewResolver(collectionManager: IEntityCollectionManager, eventSystem: IEventSystem) =
    //inherit ViewResolverSystem(eventSystem)

    //member this.collectionManager = collectionManager

    //override this.Group = new Group(typedefof<ViewComponent>) :> IGroup

    //override this.ViewHandler = { resource = ResourceLoader.Load<KinematicBody2D>("res://Player.tscn") } :> IViewHandler

    //override this.OnViewCreated(entity: IEntity, viewComponent: ViewComponent) =
        //let viewComponent2D = viewComponent.View :?> KinematicBody2D
        //let view = viewComponent2D :?> EntityBoundfs
        //view.entityCollection <- Some (this.collectionManager.GetCollectionFor(entity))
        //view.entity <- Some (entity)
        //viewComponent2D.Name <- "Player"