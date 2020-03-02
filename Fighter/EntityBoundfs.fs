namespace Fighter

open System
open Godot
open EcsRx.Entities
open EcsRx.Collections

type EntityBoundfs() = 
    inherit KinematicBody2D()

    member val entity = Option<IEntity>.None with get, set
    member val entityCollection = Option<IEntityCollection>.None with get, set

    override this._ExitTree() =
        try
            this.entityCollection.Value.RemoveEntity(this.entity.Value.Id)
        with
            | :? NullReferenceException -> printfn "Entity or Entity Collection not found"