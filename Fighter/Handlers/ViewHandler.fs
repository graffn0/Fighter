namespace Fighter.Handlers

open Fighter
open Godot
open EcsRx.Plugins.Views.ViewHandlers

type ViewHandler =
    { resource: KinematicBody2D }

    interface IViewHandler with
        member this.DestroyView(view : obj) =
            let viewObject = view :?> Node
            viewObject.Free()

        member this.SetActiveState(view : obj, isActive : bool) =
            let viewObject = view :?> Node
            viewObject.SetProcess(isActive)

        member this.CreateView() =
            this.resource.Position <- Vector2()
            this.resource.SetRotation(0.0f)
            //Applicationfs.instance.AddChild(this.resource)
            this.resource :> obj