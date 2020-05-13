namespace Fighter.Systems

open Fighter
open System.Reactive.Linq
open EcsRx.Extensions
open EcsRx.Groups
open EcsRx.Entities
open EcsRx.Plugins.ReactiveSystems.Systems
open EcsRx.Plugins.Views.Components
open Components.Combat
open Components.Node

type HealthBarSystem() =
    interface IReactToEntitySystem with
        member this.Group = 
            Group(
                typedefof<HealthComponent>,
                typedefof<HealthBarComponent>
                ) :> IGroup

        member this.ReactToEntity(entity : IEntity) =
            let healthComponent = entity.GetComponent<HealthComponent>()
            healthComponent.currentHealth.DistinctUntilChanged().Select(fun _ -> entity)

        member this.Process(entity : IEntity) =
            let healthComponent = entity.GetComponent<HealthComponent>()
            let healthBarComponent = entity.GetComponent<HealthBarComponent>()
            let health = healthComponent.currentHealth.Value

            healthBarComponent.healthBar.Value <- health