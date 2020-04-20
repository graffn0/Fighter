module Fighter.Components.Node

open Godot
open EcsRx.Components

type SpriteComponent =
    { sprite: Sprite }
    interface IComponent

type CollisionComponent =
    { collision: CollisionShape2D }
    interface IComponent