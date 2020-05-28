namespace Fighter

open System.Reactive.Linq
open System.Text.RegularExpressions
open Godot
open EcsRx.ReactiveData
open InputManager
open Blueprint

[<AbstractClass>]
type Applicationfs() =
    inherit GodotApplication()

    static let tempInstance = {
        new Applicationfs() with
        override this.ApplicationStarted() = ()
    } 
    static member val instance = tempInstance with get, set
    member this.deltaTime = new ReactiveProperty<float32>()

    override this.ApplicationStarted() =
        Applicationfs.instance <- this
        let defaultCollection = this.Application.EntityDatabase.GetCollection()
        for node in Collections.Array<Node>(this.GetChildren()) do
            match node with
            | :? KinematicBody2D | :? Camera2D ->
                let name = node.Name
                defaultCollection.CreateEntity(
                    (Regex.Replace(name, @"[^a-zA-Z\s]", ""), name)
                    |> getBlueprintByString)
                    |> ignore
            | _ -> ()

    override this._UnhandledInput(event: InputEvent) =
        if "ui_end" |> Input.IsActionPressed then
            match this
                .GetNode(new NodePath("CanvasLayer"))
                .GetNode(new NodePath("Menu")) with
            | :? HBoxContainer as menu ->
                menu.Show()
            | _ -> ()
        elif not (event.IsEcho()) && event |> inputTypeHasAction then
            sendInputMessage(event)
            this.GetTree().SetInputAsHandled()
            
    override this._Process(delta: float32) =
        this.deltaTime.Value <- delta
