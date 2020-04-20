namespace Fighter

open Godot
open System.Runtime.CompilerServices
open EcsRx.Extensions
open EcsRx.Entities
open EcsRx.Plugins.Views.Components

type private FakeNodes() =
    static member fakeBody = new KinematicBody2D()

[<Extension>]
type IEntityExtensions =
    [<Extension>]
    static member GetView(entity : IEntity) =
        match entity.GetComponent<ViewComponent>().View with
        | :? KinematicBody2D as n -> n
        | _ -> FakeNodes.fakeBody