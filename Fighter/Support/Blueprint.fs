module Fighter.Blueprint

open System.Text.RegularExpressions
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
open Components.Combat
open Components.Camera
open InputManager

type EntityType =
    | Player
    | MainCamera

type ComponentJson =
    | Input
    | Movement of speed: float32
    | State
    | Type of loadPath: string
    | Health of total: float
    | Attacks of Map<string, AttackData>
    | Camera

let private jsonResult =
    let file = new File()
    file.Open("res://Data/Entities.json", File.ModeFlags.Read) |> ignore
    let json =
        file.GetAsText()
        |> JsonConvert.DeserializeObject<Map<string, ComponentJson list>>
    file.Close()
    json

let entityTypeToString(entityType: EntityType) =
    match entityType with
    | Player -> "Player"
    | MainCamera -> "Camera"

let getBlueprintByString(entityName: string, name: string) =
    { new IBlueprint with
        member this.Apply(entity: IEntity) =
            List.iter(
                function
                | Input ->
                    let id = Regex.Replace(name, @"[^0-9\s]", "") |> int
                    let inputComponent =
                        { pendingMovement = new ReactiveProperty<Vector2>() }
                    addActor(inputComponent :> IActor, id)
                    inputComponent
                    |> entity.AddComponents
                    { id = id }
                    |> entity.AddComponents
                | Movement speed ->
                    { movement = new ReactiveProperty<Vector2>()
                      speed = speed
                      isOnFloor = new ReactiveProperty<bool>(false) }
                      |> entity.AddComponents
                | State ->
                    { currentState = new ReactiveProperty<State>(Stand) } 
                    |> entity.AddComponents
                | Type loadPath ->
                    { loadPath = loadPath
                      name = name }
                    |> entity.AddComponents
                | Health total ->
                    { currentHealth = new ReactiveProperty<float>(total)
                      totalHealth = total }
                    |> entity.AddComponents
                | Attacks attackMap ->
                    { hit = new ReactiveProperty<bool>(false)
                      attacks =
                          attackMap
                          |> Map.fold (fun keys key value -> 
                              Map.add (stringToAttackType(key)) value keys) Map.empty }
                    |> entity.AddComponents
                | Camera ->
                    { cameraRect = new ReactiveProperty<Rect2>(Rect2()) }
                    |> entity.AddComponents
                ) jsonResult.[entityName]

            ViewComponent()
            |> entity.AddComponents }

let getBlueprint(entityType: EntityType, name: string) =
    getBlueprintByString(entityTypeToString(entityType), name)