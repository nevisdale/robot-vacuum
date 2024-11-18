using Godot;
using RobotVacuum.Scripts.Garbage;

namespace RobotVacuum.Scripts;

public partial class Robot : CharacterBody2D
{
	[Export]
	private float _moveSpeed = 1f;

	[Export]
	private float _rotationSpeedRadian = 1f;

	[Export]
	private float _pushForce = 5000f;

	[Signal]
	public delegate void CapturedByEnemyEventHandler();

	[Signal]
	public delegate void CanBeCapturedByEnemyChangedEventHandler(bool changed);

	private bool _canBeCapturedByEnemy = false;

	public bool CanBeCapturedByEnemy
	{
		get
		{
			return _canBeCapturedByEnemy;
		}
		set
		{
			bool changed = _canBeCapturedByEnemy != value;
			_canBeCapturedByEnemy = value;
			if (changed)
			{
				EmitSignal(SignalName.CanBeCapturedByEnemyChanged, value);
			}
		}
	}

	private Area2D _garbage_capture_area = null;

	public override void _Ready()
	{
		_garbage_capture_area = GetNode<Area2D>("GarbageCaptureArea");
		_garbage_capture_area.AreaEntered += GarbageCaptureArea_OnAreaEntered;

		_canBeCapturedByEnemy = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		// rotate the robot
		float rotateDir = 0f;
		if (Input.IsActionPressed("rotate_left"))
		{
			rotateDir = -1f;
		}
		else if (Input.IsActionPressed("rotate_right"))
		{
			rotateDir = 1f;
		}
		if (rotateDir != 0f)
		{
			Rotate(rotateDir * _rotationSpeedRadian * (float)delta);
		}

		// move the robot
		Velocity = Vector2.Zero;
		if (Input.IsActionPressed("move_forward"))
		{
			Velocity = Vector2.Up.Rotated(Rotation) * _moveSpeed;
		}
		MoveAndSlide();

		// push physics garbage
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D kinematicCollision = GetSlideCollision(i);
			if (kinematicCollision.GetCollider() is PhysicsGarbage garbage)
			{
				garbage.Push(kinematicCollision, _pushForce * (float)delta);
			}
		}
	}

	private bool TryCaptureGarbage(IGarbage garbage)
	{
		if (garbage.CanBeCapturedByRobot())
		{
			garbage.Capture(this);
			return true;
		}
		return false;
	}

	private void GarbageCaptureArea_OnAreaEntered(Area2D area)
	{
		bool isGarbage = false;
		bool captured = false;

		IGarbage garbage = GarbageManager.GetGarbageOrNull(area);
		if (garbage != null)
		{
			isGarbage = true;
			captured = TryCaptureGarbage(garbage);
		}

		GD.Print($"{Name}. GarbageCaptureArea_OnAreaEntered: body={area.Name}, IsGarbage={isGarbage}, Captured={captured}");
	}

	public void CaptureByEnemy(Node2D enemy)
	{
		GD.Print($"{Name} is captured by {enemy.Name}");
		EmitSignal(SignalName.CapturedByEnemy);
	}

	public void MakeNotMovable()
	{
		_moveSpeed = 0f;
		_rotationSpeedRadian = 0f;
	}
}
