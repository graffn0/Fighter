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
            Group(
                typedefof<MovementComponent>,
                typedefof<ViewComponent>
                ) :> IGroup

        member this.ReactToGroup(group: IObservableGroup) =
            Observable.EveryUpdate().Select(fun _ -> group)

        member this.Process(entity : IEntity) =
            match entity.GetComponent<ViewComponent>().View with
            | :? KinematicBody2D as viewGameObject ->
                let movementComponent = entity.GetComponent<MovementComponent>()
                let mutable velocity = movementComponent.movement.Value
                movementComponent.isOnFloor.Value <- viewGameObject.IsOnFloor()
                velocity.y <- velocity.y + gravity
                movementComponent.movement.Value <- viewGameObject.MoveAndSlide(velocity, floor)
            | _ -> ()