namespace Fighter

open System.Text.RegularExpressions
open Godot
open InputManager
open Blueprint

[<AbstractClass>]
type Applicationfs() =
    inherit GodotApplication()

    static member val instance: Node = new Node() with get, set

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
        if not (event.IsEcho()) && event |> inputTypeHasAction then
            sendInputMessage(event)
            this.GetTree().SetInputAsHandled()
