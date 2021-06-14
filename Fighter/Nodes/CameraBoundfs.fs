namespace Fighter

open System
open Godot

type CameraBoundfs() = 
    inherit Camera2D()

    override this._Process(_: float32) =
        let half(value: float32) = value / 2.0f
        let calculateCenter(rect: Rect2) =
            Vector2(
                       rect.Position.x + rect.Size.x |> half,
                       rect.Position.y
                   )
        let mutable cameraRect = Rect2()
        let mutable first = true
        for node in Collections.Array<Node>(Applicationfs.instance.GetChildren()) do
            match node with
            | :? EntityBoundfs as bound ->
                if first then
                    cameraRect <- Rect2(bound.GlobalPosition, Vector2())
                    first <- false
                else
                    cameraRect <- cameraRect.Expand(bound.GlobalPosition)
            | _ -> ()
        let viewport = this.GetViewportRect()
        let center = calculateCenter(cameraRect)
        this.Offset <-
            let halfViewX = viewport.Size.x |> half
            let thirdViewY = viewport.Size.y / 3.0f
            Vector2(
                        center.x - halfViewX,
                        if center.y < thirdViewY then
                            center.y - thirdViewY
                        else
                            0.0f
                   )
        for node in Collections.Array<Node>(this.GetChildren()) do
            match node with
            | :? Node2D as node2D ->
                node2D.Position <-
                    Vector2(
                        this.Offset.x - viewport.Size.x |> half,
                        node2D.Position.y
                    )
            | _ -> ()