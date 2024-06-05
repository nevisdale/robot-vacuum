using Godot;
using System;

public partial class Robot : CharacterBody2D {
	const string MOVE_FORWARD_INPUT_ACTION = "move_forward";
	const string ROTATE_LEFT_INPUT_ACTION = "rotate_left";
	const string ROTATE_RIGHT_INPUT_ACTION = "rotate_right";

	[Export] private float movementSpeed = 1;
	[Export] private float rotationSpeedRadian = 1;

	public override void _Ready() {
		Rotate(-Mathf.Pi / 2);
	}

	public override void _PhysicsProcess(double delta) {
		int rotationDirection = 0;
		if (Input.IsActionPressed(ROTATE_LEFT_INPUT_ACTION)) {
			rotationDirection = -1;
		} else if (Input.IsActionPressed(ROTATE_RIGHT_INPUT_ACTION)) {
			rotationDirection = 1;
		}
		if (rotationDirection != 0) {
			Rotate(rotationDirection * rotationSpeedRadian * (float)delta);
		}

		Vector2 velocity = Vector2.Zero;
		if (Input.IsActionPressed(MOVE_FORWARD_INPUT_ACTION)) {
			velocity = Vector2.Right.Rotated(Rotation) * movementSpeed;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
