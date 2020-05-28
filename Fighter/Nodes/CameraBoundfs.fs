namespace Fighter

open Fighter.Components.Camera
open System
open Godot
open EcsRx.Entities
open EcsRx.Collections.Entity
open EcsRx.Extensions

type CameraBoundfs() = 
    inherit Camera2D()

    member val entity = Option<IEntity>.None with get, set
    member val entityCollection = Option<IEntityCollection>.None with get, set
    
    override this._Draw() =
        match this.entity with
        | Some entity ->
            let cameraComponent = entity.GetComponent<CameraComponent>()
            this.DrawRect(cameraComponent.cameraRect.Value, Color("#ffffff"), false)
        | _ -> ()
        
    override this._ExitTree() =
        try
            this.entityCollection.Value.RemoveEntity(this.entity.Value.Id)
        with
            | :? NullReferenceException -> printfn "Entity or Entity Collection not found"