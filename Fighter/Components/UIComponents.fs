module Fighter.Components.UI

open Godot
open EcsRx.Components
open EcsRx.ReactiveData

type MovementComponent = 
    { movement: ReactiveProperty<Vector2>
      speed: float32
      isOnFloor: ReactiveProperty<bool> }

    interface IComponent

type State =
    | Stand
    | Walk
    | Jump
    | Fall

type StateComponent =
    { currentState: ReactiveProperty<State> }

    interface IComponent

type TypeComponent = 
    { loadPath: string
      name: string }

    interface IComponent