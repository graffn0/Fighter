namespace Fighter.Components

open System
open Godot
open EcsRx.Components
open EcsRx.ReactiveData

type MovementComponent() =
    member this.Movement = new ReactiveProperty<Vector2>()
    member this.StopMovement = false

    interface IComponent

    interface IDisposable with
        member this.Dispose() =
            this.Movement.Dispose()