namespace Fighter.Systems

open Fighter
open System
open System.Reactive.Linq
open Godot
open EcsRx.Extensions
open EcsRx.Groups
open EcsRx.Groups.Observable
open EcsRx.Entities
open EcsRx.Plugins.ReactiveSystems.Systems
open Components.UI

type ChangeStateSystem() =
    interface IReactToGroupSystem with
        member this.Group = 
            Group(
                typedefof<MovementComponent>,
                typedefof<StateComponent>
                ) :> IGroup

        member this.ReactToGroup(group: IObservableGroup) =
            Observable.Interval(TimeSpan.FromSeconds(0.01)).Select(fun _ -> group)

        member this.Process(entity : IEntity) =
            let stateComponent = entity.GetComponent<StateComponent>()
            let movementComponent = entity.GetComponent<MovementComponent>()
            let velocity = movementComponent.movement.Value
            match velocity.y with
            | y when y < 0.0f ->
                stateComponent.currentState.Value <- Jump
            | _ ->
                if movementComponent.isOnFloor.Value then
                    match velocity.x with
                    | x when x <> 0.0f -> stateComponent.currentState.Value <- Walk
                    | _ -> stateComponent.currentState.Value <- Stand
                else stateComponent.currentState.Value <- Fall

type StateChangedSystem() =
    interface IReactToEntitySystem with
        member this.Group = 
            Group(
                typedefof<StateComponent>
                ) :> IGroup

        member this.ReactToEntity(entity : IEntity) =
            let stateComponent = entity.GetComponent<StateComponent>()
            stateComponent.currentState.DistinctUntilChanged().Select(fun _ -> entity)

        member this.Process(entity : IEntity) =
            let stateComponent = entity.GetComponent<StateComponent>()
            GD.Print("State changed to: " + stateComponent.currentState.Value.ToString())