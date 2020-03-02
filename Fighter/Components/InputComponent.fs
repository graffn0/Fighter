namespace Fighter

open Godot
open EcsRx.Components

type InputComponent() =
    member this.PendingMovement = new Vector2()

    interface IComponent