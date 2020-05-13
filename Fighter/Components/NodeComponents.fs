module Fighter.Components.Node

open Godot
open EcsRx.Components

type HealthBarComponent =
    { healthBar: TextureProgress }
    interface IComponent

type PlaybackComponent =
    { playback: AnimationNodeStateMachinePlayback }
    interface IComponent