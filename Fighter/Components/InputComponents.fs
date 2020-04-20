module Fighter.Components.Input

open Fighter
open Godot
open EcsRx.Components
open EcsRx.ReactiveData
open Proto
open InputManager

type InputComponent = 
    { pendingMovement: ReactiveProperty<Vector2> }

    interface IComponent

    interface IActor with
        member this.ReceiveAsync(context: IContext) =
            match context.Message with
            | :? InputMessage as x ->
                this.pendingMovement.Value <- Vector2(x.right - x.left, x.jump)
            | _ -> GD.Print("Invalid input message")
            Actor.Done