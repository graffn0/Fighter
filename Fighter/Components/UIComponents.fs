module Fighter.Components.UI

open Godot
open EcsRx.Components
open EcsRx.ReactiveData
open Fighter.Components.Combat

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
    | Attack of AttackType

let stateToString(state: State) =
    match state with
    | Stand -> "Stand"
    | Walk -> "Walk"
    | Jump -> "Jump"
    | Fall -> "Fall"
    | Attack attackType ->
        match attackType with
        | Punch -> "Punch"
        | NoAttack -> "None"

type StateComponent =
    { currentState: ReactiveProperty<State> }
    interface IComponent

type TypeComponent = 
    { loadPath: string
      name: string }
    interface IComponent

type PlayerComponent =
    { id: int }
    interface IComponent