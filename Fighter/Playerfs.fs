namespace Fighter

open Godot

type Playerfs() = 
    inherit KinematicBody2D()

    let mutable velocity = Vector2()

    override this._PhysicsProcess(delta : float32) =
        let buttonPressed = System.Convert.ToInt32(Input.IsActionPressed("ui_right")) - System.Convert.ToInt32(Input.IsActionPressed("ui_left"))
        velocity.x <- 100.0f * float32(buttonPressed)

        velocity <- this.MoveAndSlide(velocity)