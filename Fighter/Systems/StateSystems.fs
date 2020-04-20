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
open EcsRx.Plugins.Views.Components
open Components.UI
open Components.Input

type ChangeStateSystem() =
    interface IReactToGroupSystem with
        member this.Group = 
            new Group(
                typedefof<MovementComponent>,
                typedefof<StateComponent>
                ) :> IGroup

        member this.ReactToGroup(group: IObservableGroup) =
            Observable.Interval(TimeSpan.FromSeconds(0.01)).Select(fun x -> group)

        member this.Process(entity : IEntity) =
            let stateComponent = entity.GetComponent<StateComponent>()
            let movmentComponent = entity.GetComponent<MovementComponent>()
            let velocity = movmentComponent.movement.Value
            match velocity.y with
            | y when y < 0.0f ->
                stateComponent.currentState.Value <- Jump
            | _ ->
                if movmentComponent.isOnFloor.Value then
                    match velocity.x with
                    | x when x <> 0.0f -> stateComponent.currentState.Value <- Walk
                    | _ -> stateComponent.currentState.Value <- Stand
                else stateComponent.currentState.Value <- Fall

type StateChangedSystem() =
    interface IReactToEntitySystem with
        member this.Group = 
            new Group(
                typedefof<StateComponent>
                ) :> IGroup

        member this.ReactToEntity(entity : IEntity) =
            let stateComponent = entity.GetComponent<StateComponent>()
            stateComponent.currentState.DistinctUntilChanged().Select(fun x -> entity)

        member this.Process(entity : IEntity) =
            let stateComponent = entity.GetComponent<StateComponent>()
            GD.Print("State chaged to: " + stateComponent.currentState.Value.ToString())