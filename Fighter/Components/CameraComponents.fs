module Fighter.Components.Camera

open EcsRx.ReactiveData
open Godot
open EcsRx.Components

type CameraComponent =
    { cameraRect: ReactiveProperty<Rect2> }
    interface IComponent