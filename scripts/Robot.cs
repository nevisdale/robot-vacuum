using Godot;
using System;
using System.Reflection.Metadata;

namespace Objects;

public partial class Robot : CharacterBody2D {
	const string MOVE_FORWARD_INPUT_ACTION = "move_forward";
	const string ROTATE_LEFT_INPUT_ACTION = "rotate_left";
	const string ROTATE_RIGHT_INPUT_ACTION = "rotate_right";

	[Export] private float movementSpeed = 1;
	[Export] private float rotationSpeedRadian = 1;
	[Export] private float pushForce = 5000f;


	private Area2D garbageCaptureArea;

	public override void _Ready() {
		Rotate(-Mathf.Pi / 2);

		garbageCaptureArea = GetNode<Area2D>("GarbageCaptureArea");
		garbageCaptureArea.AreaEntered += GarbageCaptureArea_OnAreaEntered;
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

		for (int i = 0; i < GetSlideCollisionCount(); i++) {
			KinematicCollision2D kinematicCollision = GetSlideCollision(i);
			if (kinematicCollision.GetCollider() is RigidBody2D rb) {
				Vector2 force = pushForce * (float)delta * -kinematicCollision.GetNormal();
				Vector2 position = kinematicCollision.GetPosition() - rb.GlobalPosition;
				rb.ApplyForce(force, position);
			}
		}
	}

	private void GarbageCaptureArea_OnAreaEntered(Area2D area) {
		if (area is not Garbage.IGarbage garbage) {
			return;
		}

		if (garbage.CanBeCapturedByRobot()) {
			garbage.CaptureAndDestroy(GlobalPosition);
		}
	}
}
