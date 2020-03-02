namespace Fighter.Components

open Godot
open EcsRx.Components

type SpriteComponent =
    { SpriteObject: Sprite }
    interface IComponent