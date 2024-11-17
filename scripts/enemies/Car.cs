using Godot;
using RobotVacuum.Scripts.Garbage;

namespace RobotVacuum.Scripts.Enemies;

public partial class Car : CharacterBody2D
{
	[Export]
	private float _speed = 1000f;

	[Export]
	private float _pushForce = 5000f;

	[Export]
	private Vector2 _direction = Vector2.Zero;

	[Export]
	private bool _useRayCast = true;

	private Node2D _rayCastsLeft = null;
	private Node2D _rayCastsRight = null;


	public override void _Ready()
	{
		_rayCastsLeft = GetNode<Node2D>("RayCastLeft");
		_rayCastsRight = GetNode<Node2D>("RayCastRight");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 positionBefore = GlobalPosition;
		KinematicCollision2D collision = MoveAndCollide(_direction * _speed * (float)delta);
		GodotObject collider = collision?.GetCollider();
		if (collider is RigidBody2D rb)
		{
			if (GarbageManager.IsGarbage(rb))
			{
				Vector2 force = _pushForce * -collision.GetNormal() * (float)delta;
				Vector2 pos = collision.GetPosition() - rb.GlobalPosition;
				rb.ApplyForce(force, pos);
			}
		}

		if (positionBefore.IsEqualApprox(GlobalPosition))
		{
			TryToPushCan(delta);
		}
	}

	public void SetDirectionTo(Vector2 target)
	{
		_direction = target.Normalized();
	}

	// if the car stuck, try to push the can
	// it could be if car is stuck between a wall and a can
	private void TryToPushCan(double delta)
	{
		if (!_useRayCast || _direction == Vector2.Zero)
		{
			return;
		}

		Node2D rayCastGroup = _rayCastsLeft;
		if (_direction == Vector2.Right)
		{
			rayCastGroup = _rayCastsRight;
		}

		int rayCastCount = rayCastGroup.GetChildCount();
		for (int i = 0; i < rayCastCount; i++)
		{
			RayCast2D rayCast = rayCastGroup.GetChild<RayCast2D>(i);
			rayCast.ForceRaycastUpdate();
			if (!rayCast.IsColliding())
			{
				continue;
			}

			GodotObject collider = rayCast.GetCollider();
			if (collider is not Can can)
			{
				continue;
			}
			Vector2 force = _pushForce * _direction * (float)delta;
			can.ApplyCentralForce(force);
			GD.Print($"{can.Name} has been pushed by {Name}");
			break;
		}
	}
}
