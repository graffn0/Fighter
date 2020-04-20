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
            let name = node.Name
            let nameType = Regex.Replace(name, @"[^a-zA-Z\s]", "")
            match stringToEntityType(nameType) with
            | Some t ->
                defaultCollection.CreateEntity(getBlueprint(t, name)) |> ignore
            | _ -> ()

    override this._UnhandledInput(event: InputEvent) =
        if not (event.IsEcho()) && event |> inputTypeHasAction then
            sendInputMessage(event)
            this.GetTree().SetInputAsHandled()
