namespace Fighter.Systems

open Fighter
open System
open System.Reactive.Linq
open System.Reactive.Concurrency
open System.Threading
open Fighter.Components.Input
open Godot
open EcsRx.Extensions
open EcsRx.Groups
open EcsRx.Groups.Observable
open EcsRx.Entities
open EcsRx.Plugins.ReactiveSystems.Systems
open EcsRx.Plugins.Views.Components
open Components.Camera

type CameraSystem() =
    interface IReactToGroupSystem with
        member this.Group = 
            Group(
                typedefof<ViewComponent>,
                typedefof<CameraComponent>
                ) :> IGroup

        member this.ReactToGroup(group: IObservableGroup) =
            Observable.EveryUpdate().Select(fun _ -> group)

        member this.Process(entity : IEntity) =
            let viewComponent = entity.GetComponent<ViewComponent>()
            let cameraComponent = entity.GetComponent<CameraComponent>()
            let calculateCenter(rect: Rect2) =
                Vector2(
                           rect.Position.x + rect.Size.x / 2.0f,
                           rect.Position.y
                       )
            let mutable cameraRect = Rect2()
            let mutable first = true
            for node in Collections.Array<Node>(Applicationfs.instance.GetChildren()) do
                match node with
                | :? EntityBoundfs as bound ->
                    match bound.entity with
                    | Some playerEntity ->
                        if playerEntity.HasComponent<InputComponent>() then
                            if first then
                                cameraRect <- Rect2(bound.GlobalPosition, Vector2())
                                first <- false
                            else
                                cameraRect <- cameraRect.Expand(bound.GlobalPosition)
                    | _ -> ()
                | _ -> ()
                
            match viewComponent.View with
            | :? Camera2D as camera ->
                let viewport = camera.GetViewportRect()
                let center = calculateCenter(cameraRect)
                camera.Offset <-
                    let halfViewX = viewport.Size.x / 2.0f
                    let thirdViewY = viewport.Size.y / 3.0f
                    Vector2(
                                center.x - halfViewX,
                                if center.y < thirdViewY then
                                    center.y - thirdViewY
                                else
                                    0.0f
                           )
                
                cameraComponent.cameraRect.Value <- cameraRect
                camera.Update()
            | _ -> ()