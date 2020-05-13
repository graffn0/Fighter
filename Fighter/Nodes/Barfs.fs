namespace Fighter

open System
open Godot
open EcsRx.Entities
open EcsRx.Collections.Entity

type EntityBoundfs() = 
    inherit KinematicBody2D()

    member val entity = Option<IEntity>.None with get, set
    member val entityCollection = Option<IEntityCollection>.None with get, set

    member this.OnAttackHitAreaEntered(area: Area2D) =
        if area.IsInGroup("hit") then
            GD.Print("Hit")

    override this._ExitTree() =
        try
            this.entityCollection.Value.RemoveEntity(this.entity.Value.Id)
        with
            | :? NullReferenceException -> printfn "Entity or Entity Collection not found"