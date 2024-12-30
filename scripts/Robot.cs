using Godot;
using RobotVacuum.Scripts.Audio;
using RobotVacuum.Scripts.Enemies;
using RobotVacuum.Scripts.Garbage;
using System;

namespace RobotVacuum.Scripts;

public partial class Robot : CharacterBody2D, IElectricityReceiver
{
	private const float LIGHT_ENERGY_MIN = 1f;
	private const float LIGHT_ENERGY_MAX = 15f;

	[Export]
	private float _moveSpeed = 1f;
	[Export]
	private float _rotationSpeedRadian = 1f;
	[Export]
	private float _pushForce = 5000f;
	[Export]
	private float _playSoundPhysicsGarbageDelay = 0.2f;

	[Signal] public delegate void CapturedByEnemyEventHandler();
	[Signal] public delegate void CanBeCapturedByEnemyChangedEventHandler(bool changed);

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

	private AnimationPlayer _animationPlayer = null;
	private Area2D _garbage_capture_area = null;
	private PointLight2D _pointLightGreen = null;
	private PointLight2D _pointLightRed = null;

	private int _dangerCount = 0;
	private Tween _tweenLight = null;

	public override void _Ready()
	{
		_garbage_capture_area = GetNode<Area2D>("GarbageCaptureArea");
		_pointLightGreen = GetNode<PointLight2D>("PointLightGreen");
		_pointLightRed = GetNode<PointLight2D>("PointLightRed");

		_garbage_capture_area.AreaEntered += GarbageCaptureArea_OnAreaEntered;

		_canBeCapturedByEnemy = true;

		OutDangerAreaTweenAnimation();
	}

	private string _rotateLeftAction = "rotate_left";
	private string _rotateRightAction = "rotate_right";
	private string _moveForwardAction = "move_forward";
	private string[] _allActions = new string[] { "rotate_left", "rotate_right", "move_forward" };


	private static void ShuffleArray(string[] array)
	{
		Random random = new Random();
		for (int i = array.Length - 1; i > 0; i--)
		{
			int j = random.Next(0, i + 1);
			(array[j], array[i]) = (array[i], array[j]);
		}

		// make sure that new robot control will be different from the starting control
		if (array[2] == "move_forward")
		{
			(array[0], array[2]) = (array[2], array[0]);
		}
	}

	private void ChangeControl()
	{
		ShuffleArray(_allActions);
		_rotateLeftAction = _allActions[0];
		_rotateRightAction = _allActions[1];
		_moveForwardAction = _allActions[2];
	}

	public override void _PhysicsProcess(double delta)
	{
		// rotate the robot
		float rotateDir = 0f;
		if (Input.IsActionPressed(_rotateLeftAction))
		{
			rotateDir = -1f;
		}
		else if (Input.IsActionPressed(_rotateRightAction))
		{
			rotateDir = 1f;
		}
		if (rotateDir != 0f)
		{
			Rotate(rotateDir * _rotationSpeedRadian * (float)delta);
		}

		// move the robot
		Velocity = Vector2.Zero;
		if (Input.IsActionPressed(_moveForwardAction))
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
				if (garbage.IsMovingByCar())
				{
					// physics garbage is moving by car,
					// so the robot is captured by the garbage
					CaptureByEnemy(garbage);
					return;
				}

				// avoid playing sound when a robot is pushing the garbage continuously
				long now = DateTime.Now.Ticks;
				long delay = TimeSpan.FromSeconds(_playSoundPhysicsGarbageDelay).Ticks;
				if (now - garbage.TouchedByRobotLastTime >= delay)
				{
					AudioManager.Instance.PlaySound_RobotPushGarbage();
				}
				garbage.TouchedByRobotLastTime = now;

				garbage.Push(kinematicCollision, _pushForce * (float)delta);
				garbage.SendElectricityIfPossible(this);
			}
		}
	}

	private bool TryCaptureGarbage(IGarbage garbage)
	{
		if (garbage.CanBeCapturedByRobot())
		{
			AudioManager.Instance.PlaySound_RobotCaptureGarbage();
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
		switch (enemy)
		{
			case Cat:
				AudioManager.Instance.PlaySound_CatCaptureRobot();
				break;
			case Car:
				AudioManager.Instance.PlaySound_CarCaptureRobot();
				break;
		}

		GD.Print($"{Name} is captured by {enemy.Name}");
		EmitSignal(SignalName.CapturedByEnemy);
	}

	public void MakeNotMovable()
	{
		_moveSpeed = 0f;
		_rotationSpeedRadian = 0f;
	}

	public void InDangerArea()
	{
		_dangerCount += 1;
		if (_dangerCount > 0)
		{
			InDangerAreaTweenAnimation();
		}
	}

	public void OutDangerArea()
	{
		_dangerCount -= 1;
		if (_dangerCount == 0)
		{
			OutDangerAreaTweenAnimation();
		}
	}

	public void DisableLight()
	{
		_pointLightGreen.Visible = false;
		_pointLightRed.Visible = false;
	}

	private void InDangerAreaTweenAnimation()
	{
		_tweenLight?.Stop();

		_tweenLight = CreateTween();
		_tweenLight.TweenProperty(_pointLightGreen, "energy", LIGHT_ENERGY_MIN, 0.5);

		// run this tween little bit faster,
		// it shows that the robot is in danger area
		_tweenLight = CreateTween();
		_tweenLight.SetLoops();
		_tweenLight.TweenProperty(_pointLightRed, "energy", LIGHT_ENERGY_MAX, 0.2);
		_tweenLight.TweenProperty(_pointLightRed, "energy", LIGHT_ENERGY_MIN, 0.2);
	}

	private void OutDangerAreaTweenAnimation()
	{
		_tweenLight?.Stop();

		_tweenLight = CreateTween();
		_tweenLight.TweenProperty(_pointLightRed, "energy", LIGHT_ENERGY_MIN, 0.5);
		_tweenLight.TweenProperty(_pointLightGreen, "energy", LIGHT_ENERGY_MAX, 0.5);
	}

	public void ReceiveElectricity()
	{
		AudioManager.Instance.PlaySound_GetElectricity();
		ChangeControl();
	}
}
