using System.Collections.Generic;
using Godot;
using RobotVacuum.Scripts.Garbage;

namespace RobotVacuum.Scripts.Enemies;

public partial class Car : CharacterBody2D
{
	[Export] private float _speed = 1000f;
	[Export] private float _pushForce = 5000f;
	[Export] private Vector2 _direction = Vector2.Zero;
	[Export] private bool _useRayCast = true;
	[Export] private float _wheelRotationSpeed = 1f;

	private Node2D _rayCastsLeft = null;
	private Node2D _rayCastsRight = null;
	private Area2D _dangerArea = null;
	private readonly List<Sprite2D> _wheels = new();
	private float _wheelSpeed = 0.5f;

	public override void _Ready()
	{
		_rayCastsLeft = GetNode<Node2D>("RayCastLeft");
		_rayCastsRight = GetNode<Node2D>("RayCastRight");
		_dangerArea = GetNode<Area2D>("DangerArea");

		Node2D _wheelNode = GetNode<Node2D>("Wheels");
		for (int i = 0; i < _wheelNode.GetChildCount(); i++)
		{
			_wheels.Add(_wheelNode.GetChild<Sprite2D>(i));
		}

		_dangerArea.BodyEntered += DangerArea_OnBodyEntered;
		_dangerArea.BodyExited += DangerArea_OnBodyExited;
	}

	public override void _PhysicsProcess(double delta)
	{
		// if the car is not moving, then do nothing
		// otherwise, robot can move a car using physics garbage
		if (_direction == Vector2.Zero)
		{
			TryPushGarbage(delta);
			return;
		}

		Vector2 positionBefore = GlobalPosition;
		KinematicCollision2D collision = MoveAndCollide(_direction * _speed * (float)delta);
		GodotObject collider = collision?.GetCollider();
		if (collider is PhysicsGarbage garbage)
		{
			// push psysics garbage
			garbage.Push(collision, _pushForce * (float)delta);
			if (_direction != Vector2.Zero)
			{
				garbage.TouchedByCar();
			}
		}
		else if (collider is Robot robot)
		{
			// if car is moving and touches the robot,
			// so the robot is captured by the car
			robot.MakeNotMovable();
			robot.CaptureByEnemy(this);
		}

		// TODO: add Wall object
		// now only Walls might be detected TileMapLayer
		else if (collider is StaticBody2D || collider is TileMapLayer)
		{
			// if car is moving and touches the wall,
			// or other static body objects, then stop the car
			_direction = Vector2.Zero;
			_wheelSpeed = 0;
		}

		if (positionBefore.IsEqualApprox(GlobalPosition))
		{
			// the car may be stuck
			// try to push the garbage in order to move the car
			TryPushGarbage(delta);
		}

		// rotate wheels
		foreach (Sprite2D wheel in _wheels)
		{
			wheel.Rotation += _wheelSpeed * (float)delta;
		}
	}

	public void SetDirectionTo(Vector2 target)
	{
		_direction = target.Normalized();
		if (_direction == Vector2.Left)
		{
			_wheelSpeed = -_wheelRotationSpeed;
		}
		else if (_direction == Vector2.Right)
		{
			_wheelSpeed = _wheelRotationSpeed;
		}
	}

	// if the car stuck, try to push the can
	// it could be if car is stuck between a wall and a can
	private void TryPushGarbage(double delta)
	{
		if (!_useRayCast)
		{
			return;
		}

		if (_direction == Vector2.Left)
		{
			TryPushGarbageFrom(_rayCastsLeft, Vector2.Left, delta);
		}
		else if (_direction == Vector2.Right)
		{
			TryPushGarbageFrom(_rayCastsRight, Vector2.Right, delta);
		}
		else
		{
			// means the car is not moving
			// so push every garbage in all directions
			TryPushGarbageFrom(_rayCastsLeft, Vector2.Left, delta);
			TryPushGarbageFrom(_rayCastsRight, Vector2.Right, delta);
		}
	}

	private void TryPushGarbageFrom(Node2D group, Vector2 forceDirection, double delta)
	{
		int rayCastCount = group.GetChildCount();
		for (int i = 0; i < rayCastCount; i++)
		{
			RayCast2D rayCast = group.GetChild<RayCast2D>(i);
			rayCast.ForceRaycastUpdate();
			if (!rayCast.IsColliding())
			{
				continue;
			}

			GodotObject collider = rayCast.GetCollider();
			if (collider is not PhysicsGarbage garbage)
			{
				continue;
			}
			// push the garbage not so hard
			const float pushKoef = 0.01f;
			Vector2 force = _pushForce * pushKoef * forceDirection * (float)delta;
			garbage.ApplyCentralForce(force);
			GD.Print($"{garbage.Name} has been pushed by {Name}");
			break;
		}
	}

	private void DangerArea_OnBodyEntered(Node2D body)
	{
		if (body is Robot robot)
		{
			robot.InDangerArea();
		}
	}

	private void DangerArea_OnBodyExited(Node2D body)
	{
		if (body is Robot robot)
		{
			robot.OutDangerArea();
		}
	}
}
