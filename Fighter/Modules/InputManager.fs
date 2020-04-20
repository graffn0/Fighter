module Fighter.InputManager

open System
open Godot
open Proto

type InputType =
    | Right
    | Left
    | Jump

[<Struct>]
type InputMessage = 
    { left: float32 
      right: float32
      jump: float32 }

let private actorManager = new ActorSystem() |> RootContext
let mutable private inputActors = List<PID>.Empty
let private inputMap =
    let mutable map = Map.empty
    for action in InputMap.GetActions() |> Collections.Array<string> do
        for key in
            InputMap.GetActionList(action)
            |> Collections.Array<InputEvent> do
                map <- map.Add(key.AsText(), action)
    map

let addActor(actor: IActor) =
    let props = Props.FromProducer(fun () -> actor)
    inputActors <- actorManager.Spawn(props) :: inputActors

let inputTypeHasAction(event: InputEvent) =
    event.AsText() |> inputMap.ContainsKey

let sendInputMessage(event: InputEvent) =
    let inputTypeToAction(input: InputType) =
        match input with
        | Right -> "ui_right"
        | Left -> "ui_left"
        | Jump -> "jump"

    let isPressedToFloat(input: InputType) =
        input
        |> inputTypeToAction
        |> Input.IsActionPressed
        |> Convert.ToInt16
        |> float32

    inputActors
    |> List.iter(fun actor ->
        actorManager.Send(actor, {
            right = isPressedToFloat(Right)
            left = isPressedToFloat(Left)
            jump = isPressedToFloat(Jump) 
            })
        )