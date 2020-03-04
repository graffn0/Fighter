namespace Fighter

open Fighter.Components
open Godot
open System.Runtime.CompilerServices
open EcsRx.Extensions
open EcsRx.Entities
open EcsRx.Plugins.Views.Components

[<Extension>]
type IEntityExtensions =
    [<Extension>]
    static member GetView(entity : IEntity) = entity.GetComponent<ViewComponent>().View :?> KinematicBody2D