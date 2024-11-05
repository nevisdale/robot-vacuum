using Godot;

namespace RobotVacuum.Scripts.Enemies;

public partial class Cat : Node2D
{
	[Export]
	private float _move_speed = 400f;

	private Area2D _robot_capture_area;
	private Sprite2D _sprite;

	private PathFollow2D _path_follow;

	// needs to remember the previous position to flip the sprite
	// instead of using PathFollow2D's rotation
	// because it rotates the Cat in x and y axis,
	// but we only need to flip the sprite in x axis
	private Vector2 _prev_global_position;

	private Robot _targetRobot;
	private Tween _captureTween;

	public override void _Ready()
	{
		_robot_capture_area = GetNode<Area2D>("RobotCaptureArea");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_path_follow = GetParentOrNull<PathFollow2D>();

		if (_path_follow == null)
		{
			GD.Print($"{Name} does not have a PathFollow2D parent. do not follow the path.");
		}

		_prev_global_position = GlobalPosition;

		_robot_capture_area.BodyEntered += RobotCaptureArea_OnBodyEntered;
		_robot_capture_area.BodyExited += RobotCaptureArea_OnBodyExited;
	}

	public override void _Process(double delta)
	{
		if (_path_follow != null)
		{
			_path_follow.Progress += _move_speed * (float)delta;

			// flip the Sprite2D in x axis
			// if the Cat is moving to the left
			_sprite.FlipH = _prev_global_position.X > GlobalPosition.X;
		}
		_prev_global_position = GlobalPosition;
	}

	private void RobotCaptureArea_OnBodyEntered(Node body)
	{
		_targetRobot = body as Robot;
		if (_targetRobot == null)
		{
			GD.Print($"{Name} captured a non-Robot object {body.Name}. ignore it.");
			return;
		}

		if (_targetRobot.CanBeCapturedByEnemy)
		{
			GD.Print($"{Name} captured a {_targetRobot.Name}.");
			CaptureRobot();
			return;
		}

		// the Robot is not capturable by the Cat right now
		// but it may change in the future
		_targetRobot.CanBeCapturedByEnemyChanged += TargetRobot_CanBeCapturedByEnemyChanged;
		GD.Print($"{Name} may capture a {_targetRobot.Name} in the future.");
	}

	private void RobotCaptureArea_OnBodyExited(Node body)
	{
		if (_captureTween != null)
		{
			// tween is running. the Cat is moving to the robot
			return;
		}

		if (_targetRobot != null)
		{
			_targetRobot.CanBeCapturedByEnemyChanged -= TargetRobot_CanBeCapturedByEnemyChanged;
			_targetRobot = null;

			GD.Print($"{Name} released a {body.Name}.");
		}
	}

	private void TargetRobot_CanBeCapturedByEnemyChanged(bool canBeCaptured)
	{
		if (canBeCaptured)
		{
			CaptureRobot();
		}
	}

	private async void CaptureRobot()
	{
		if (_captureTween != null)
		{
			await ToSignal(_captureTween, "finished");
			return;
		}

		// because tween uses the Robot global position to move the Cat
		// and we don't want to change Robot position
		_targetRobot.MakeNotMovable();

		// stop following the path
		// in order to move to the robot
		_path_follow = null;

		// rotate the Cat to the robot
		_sprite.FlipH = GlobalPosition.X > _targetRobot.GlobalPosition.X;

		const float TWEEN_DURATION = 0.2f;
		_captureTween = CreateTween();
		_captureTween.Finished += () =>
		{
			_targetRobot.CaptureByEnemy(this);
			_targetRobot = null;
		};
		_captureTween.TweenProperty(this, "global_position", _targetRobot.GlobalPosition, TWEEN_DURATION);
	}
}