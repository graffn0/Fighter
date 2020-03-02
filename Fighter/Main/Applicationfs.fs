namespace Fighter

open Fighter.Components
open Godot
open EcsRx.Extensions
open EcsRx.Godot
open EcsRx.Plugins.Views.Components

[<AbstractClass>]
type Applicationfs() =
    inherit GodotApplication()

    //static member val instance: Node = new Node() with get, set

    override this.ApplicationStarted() =
        //Applicationfs.instance <- this
        let defaultCollection = this.EntityCollectionManager.GetCollection()
        let entity = defaultCollection.CreateEntity()
        entity.AddComponent<ViewComponent>() |> ignore
        //entity.AddComponents(new MovementComponent(), new ViewComponent())

        let playerView = entity.GetComponent(typedefof<ViewComponent2D>)
        let viewGameObject = entity.GetView()

        viewGameObject.Position <- Vector2(600.0f, 400.0f)
        GD.Print(viewGameObject.Position)