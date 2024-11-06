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
	private Vector2 _initDirection = Vector2.Zero;

	private Vector2 _direction;

	public override void _Ready()
	{
		_direction = _initDirection;
	}

	public override void _PhysicsProcess(double delta)
	{
		KinematicCollision2D collision = MoveAndCollide(_direction * _speed * (float)delta);
		if (collision?.GetCollider() is RigidBody2D rb)
		{
			if (GarbageManager.IsGarbage(rb))
			{
				Vector2 force = _pushForce * -collision.GetNormal() * (float)delta;
				Vector2 pos = collision.GetPosition() - rb.GlobalPosition;
				rb.ApplyForce(force, pos);
			}
		}
	}

	public void SetDirectionTo(Vector2 target)
	{
		_direction = target.Normalized();
	}
}
