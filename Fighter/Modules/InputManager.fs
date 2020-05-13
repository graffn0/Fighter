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
let mutable private inputActors = List<int * PID>.Empty
let private inputMap =
    let mutable map = Map.empty
    for action in InputMap.GetActions() |> Collections.Array<string> do
        for key in
            InputMap.GetActionList(action)
            |> Collections.Array<InputEvent> do
                map <- map.Add(key.AsText(), action)
    map

let addActor(actor: IActor, player: int) =
    let props = Props.FromProducer(fun () -> actor)
    inputActors <- (player, actorManager.Spawn(props)) :: inputActors

let inputTypeHasAction(event: InputEvent) =
    event.AsText() |> inputMap.ContainsKey

let sendInputMessage(event: InputEvent) =
    let inputTypeToAction(input: InputType, player: int) =
        match player with
        | 1 ->
            match input with
            | Right -> "Player 1 right"
            | Left -> "Player 1 left"
            | Jump -> "Player 1 jump"
        | 2 ->
            match input with
            | Right -> "Player 2 right"
            | Left -> "Player 2 left"
            | Jump -> "Player 2 jump"
        | _ -> "ui_cancel"

    let isPressedToFloat(input: InputType, player: int) =
        (input, player)
        |> inputTypeToAction
        |> Input.IsActionPressed
        |> Convert.ToInt16
        |> float32

    inputActors
    |> List.iter(fun actorTuple ->
        let id, actor = actorTuple
        actorManager.Send(actor, {
            right = isPressedToFloat(Right, id)
            left = isPressedToFloat(Left, id)
            jump = isPressedToFloat(Jump, id) 
            })
        )