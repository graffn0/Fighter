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
            let scheduler = SynchronizationContextScheduler(SynchronizationContext.Current)
            Observable.Interval(TimeSpan.FromSeconds(0.01), scheduler).Select(fun _ -> group)

        member this.Process(entity : IEntity) =
            let viewComponent = entity.GetComponent<ViewComponent>()
            let cameraComponent = entity.GetComponent<CameraComponent>()
            let calculateCenter(rect: Rect2) =
                Vector2(
                           rect.Position.x + rect.Size.x / 2.0f,
                           rect.Position.y / 2.0f
                       )
            cameraComponent.cameraRect.Value <- Rect2()
            for node in Collections.Array<Node>(Applicationfs.instance.GetChildren()) do
                match node with
                | :? EntityBoundfs as bound ->
                    match bound.entity with
                    | Some playerEntity ->
                        if playerEntity.HasComponent<InputComponent>() then
                            cameraComponent.cameraRect.Value <-
                                cameraComponent.cameraRect.Value.Expand(bound.GlobalPosition)
                    | _ -> ()
                | _ -> ()
            
            match viewComponent.View with
            | :? Camera2D as camera ->
                camera.Offset <- calculateCenter(cameraComponent.cameraRect.Value)
            | _ -> ()