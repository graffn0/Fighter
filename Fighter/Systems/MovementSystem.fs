namespace Fighter.Systems

open Fighter
open System
open System.Reactive.Linq
open System.Reactive.Concurrency
open System.Threading
open Godot
open EcsRx.Extensions
open EcsRx.Groups
open EcsRx.Groups.Observable
open EcsRx.Entities
open EcsRx.Plugins.ReactiveSystems.Systems
open EcsRx.Plugins.Views.Components
open Components.UI

type MovementSystem() =
    let gravity = 5.0f
    let floor = Nullable(Vector2(0.0f, -1.0f))

    interface IReactToGroupSystem with
        member this.Group = 
            new Group(
                typedefof<MovementComponent>,
                typedefof<ViewComponent>
                ) :> IGroup

        member this.ReactToGroup(group: IObservableGroup) =
            let scheduler = new SynchronizationContextScheduler(SynchronizationContext.Current)
            Observable.Interval(TimeSpan.FromSeconds(0.01), scheduler).Select(fun x -> group)

        member this.Process(entity : IEntity) =
            let viewGameObject = entity.GetView()
            let movmentComponent = entity.GetComponent<MovementComponent>()
            let mutable velocity = movmentComponent.movement.Value
            movmentComponent.isOnFloor.Value <- viewGameObject.IsOnFloor()
            velocity.y <- velocity.y + gravity
            movmentComponent.movement.Value <- viewGameObject.MoveAndSlide(velocity, floor)