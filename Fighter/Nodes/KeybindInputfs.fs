namespace Fighter

open System
open System.Collections.Generic
open System.Linq
open Godot

type KeybindInputfs() as this = 
    inherit HBoxContainer()

    [<Export>]
    let labelText = String.Empty
    [<Export>]
    let inputMapKey = String.Empty

    let labelNode = lazy(this.GetNode(new NodePath("Label")) :?> Label)
    let lineEditNode = lazy(this.GetNode(new NodePath("LineEdit")) :?> LineEdit)
    let resetButtonNode = lazy(this.GetNode(new NodePath("ResetButton")) :?> Button)

    let defaultKeybinds = Dictionary<String, List<InputEvent>>()

    let Actions = InputMap.GetActions() |> Collections.Array<string>

    let eraseActionEvents(key: String) =
        let actions =
            InputMap.GetActionList(key)
            |> Collections.Array<InputEvent>
        for ev in actions do
            if InputMap.HasAction(key) then
                InputMap.ActionEraseEvent(key, ev)

    let getKeyText(ev: InputEventKey) =
        OS.GetScancodeString(ev.Scancode)

    let getFirstEventKeyText(key: String) =
        let actions = InputMap.GetActionList(key)
        match actions.[0] with
        | :? InputEventKey as ev -> getKeyText(ev)
        | _ -> String.Empty

    let getDefaultInputEvent(action: String) =
        match defaultKeybinds.[action].Count with
        | x when x >= 0 -> Some defaultKeybinds.[action].[0]
        | _ -> None

    let initDefaultKeybinds() =
        for action in Actions do
            let inputEventsArray =
                InputMap.GetActionList(action)
                |> Collections.Array<InputEvent>
            let inputEvents = inputEventsArray.ToList()
            defaultKeybinds.Add(action, inputEvents)

    let onGuiInput(ev: InputEvent) =
        match ev with
        | :? InputEventKey as e ->
            eraseActionEvents(inputMapKey)
            InputMap.ActionAddEvent(inputMapKey, ev)
            lineEditNode.Value.Text <- getKeyText(e)
            this.AcceptEvent()
        | _ -> ()

    let onResetButtonPressed() =
        eraseActionEvents(inputMapKey)
        match getDefaultInputEvent(inputMapKey) with
        | Some ev ->
            InputMap.ActionAddEvent(inputMapKey, ev)
            lineEditNode.Value.Text <- getKeyText(ev :?> InputEventKey)
        | None -> ()

    override this._Ready() =
        initDefaultKeybinds()
        lineEditNode.Value.Text <- getFirstEventKeyText(inputMapKey)
        labelNode.Value.Text <- labelText
        lineEditNode.Value.Connect("gui_input", this, "onGuiInput") |> ignore
        resetButtonNode.Value.Connect("pressed", this, "onResetButtonPressed") |> ignore