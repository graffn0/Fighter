module Fighter.Components.Combat

open Godot
open EcsRx.Components
open EcsRx.ReactiveData

type HealthComponent = 
    { currentHealth: ReactiveProperty<float>
      totalHealth: float }
    interface IComponent

[<Struct>]
type AttackData =
    { damage: float }

type AttackType =
    | Punch
    | NoAttack

let stringToAttackType(value: string) =
    match value with
    | "Punch" -> Punch
    | _ -> NoAttack

type AttacksComponent = 
    { hit: ReactiveProperty<bool>
      attacks: Map<AttackType, AttackData> }
    interface IComponent