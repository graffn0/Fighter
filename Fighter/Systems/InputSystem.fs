namespace Fighter.Systems

open Fighter
open System.Reactive.Linq
open EcsRx.Extensions
open EcsRx.Groups
open EcsRx.Entities
open EcsRx.Plugins.ReactiveSystems.Systems
open EcsRx.Plugins.Views.Components
open Components.UI
open Components.Input

type InputSystem() =
    let jumpHeight = -300.0f

    interface IReactToEntitySystem with
        member this.Group = 
            new Group(
                typedefof<MovementComponent>,
                typedefof<InputComponent>
                ) :> IGroup

        member this.ReactToEntity(entity : IEntity) =
            let inputComponent = entity.GetComponent<InputComponent>()
            inputComponent.pendingMovement.DistinctUntilChanged().Select(fun x -> entity)

        member this.Process(entity : IEntity) =
            let inputComponent = entity.GetComponent<InputComponent>()
            let movmentComponent = entity.GetComponent<MovementComponent>()
            let input = inputComponent.pendingMovement.Value
            let mutable velocity = movmentComponent.movement.Value

            velocity.x <- input.x * movmentComponent.speed
            if movmentComponent.isOnFloor.Value then
                velocity.y <- input.y * jumpHeight
            movmentComponent.movement.Value <- velocity