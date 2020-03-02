namespace Fighter.Systems

open Fighter
open Fighter.Components
open System
open System.Reactive.Linq
open Godot
open EcsRx.Extensions
open EcsRx.Groups
open EcsRx.Entities
open EcsRx.Plugins.ReactiveSystems.Systems

type MovementSystem() =
    interface IReactToEntitySystem with
        member this.Group = new Group(typedefof<MovementComponent>, typedefof<ViewComponent2D>) :> IGroup

        member this.ReactToEntity(entity : IEntity) =
            Observable.Interval(TimeSpan.FromSeconds(0.1)).Select(fun x -> entity)
            //let movmentComponent : MovementComponent = entity.GetComponent<MovementComponent>()
            //movmentComponent.Movement.DistinctUntilChanged().Where(fun x -> x.Length() < 0.0001f).Select(fun x -> entity)

        member this.Process(entity : IEntity) =
            let viewGameObject = entity.GetView()
            let mutable movmentComponent : MovementComponent = entity.GetComponent<MovementComponent>()
            let mutable velocity = movmentComponent.Movement.Value
            let buttonPressed = System.Convert.ToInt32(Input.IsActionPressed("ui_right")) - System.Convert.ToInt32(Input.IsActionPressed("ui_left"))
            velocity.x <- 100.0f * float32(buttonPressed)

            velocity <- viewGameObject.MoveAndSlide(velocity)

    //interface IReactToGroupSystem with
        //member this.ReactToGroup(group : IObservableGroup) =
        //    Observable.Interval(TimeSpan.FromSeconds(2.0)).Select(fun x -> group)
        //member this.Process(entity : IEntity) =
            //let mutable movmentComponent : MovementComponent = entity.GetComponent<MovementComponent>()
            //let mutable velocity = movmentComponent.Movement.Value
            //if (velocity.Length() < 0.0001f) then
            //    printfn "test"

            //let buttonPressed = System.Convert.ToInt32(Input.IsActionPressed("ui_right")) - System.Convert.ToInt32(Input.IsActionPressed("ui_left"))
            //velocity <- Vector2(100.0f * float32(buttonPressed), 0.0f)

            //velocity <- this.MoveAndSlide(velocity)