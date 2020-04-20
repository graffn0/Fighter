[<AutoOpen>]
module ViewHandlers

open Godot
open EcsRx.Plugins.Views.ViewHandlers

let viewHandler(resource: Node) =
    { new IViewHandler with
        member this.DestroyView(view : obj) =
            match view with
            | :? Node as viewObject -> viewObject.Free()
            | _ -> ()

        member this.SetActiveState(view : obj, isActive : bool) =
            match view with
            | :? Node as viewObject -> viewObject.SetProcess(isActive)
            | _ -> ()

        member this.CreateView() =
            resource :> obj }