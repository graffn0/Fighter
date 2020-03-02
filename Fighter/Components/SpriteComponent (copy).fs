namespace Fighter.Components

open Godot
open EcsRx.Components

type SpriteComponent2 =
    { SpriteObject: Sprite }
    interface IComponent