module Fighter.Blueprint

open Godot
open EcsRx.Blueprints
open EcsRx.Entities
open EcsRx.Extensions
open EcsRx.Plugins.Views.Components
open EcsRx.ReactiveData
open Newtonsoft.Json
open Proto
open Components.UI
open Components.Input
open InputManager

type EntityType =
    | Player

type ComponentJson =
    | Input
    | Movement of speed: float32
    | State of State
    | Type of loadPath: string

let private jsonResult =
    let file = new File()
    file.Open("res://Entities.json", File.ModeFlags.Read) |> ignore
    let json =
        file.GetAsText()
        |> JsonConvert.DeserializeObject<Map<string, ComponentJson list>>
    file.Close()
    json

let entityTypeToString(entityType: EntityType) =
    match entityType with
    | Player -> "Player"

let getBlueprint(entityType: EntityType, name: string) =
    { new IBlueprint with
        member this.Apply(entity: IEntity) =
            List.iter(fun (x: ComponentJson) ->
                match x with
                | Input ->
                    let inputComponent =
                        { pendingMovement = new ReactiveProperty<Vector2>() }
                    addActor(inputComponent :> IActor)
                    inputComponent
                    |> entity.AddComponents
                | Movement speed ->
                    { movement = new ReactiveProperty<Vector2>()
                      speed = speed
                      isOnFloor = new ReactiveProperty<bool>(false) }
                    |> entity.AddComponents
                | State state ->
                    { currentState = new ReactiveProperty<State>(state) }
                    |> entity.AddComponents
                | Type loadPath ->
                    { loadPath = loadPath
                      name = name }
                    |> entity.AddComponents
                ) jsonResult.[entityTypeToString(entityType)]

            new ViewComponent()
            |> entity.AddComponents }