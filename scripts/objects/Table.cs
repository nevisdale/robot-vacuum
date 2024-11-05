using Godot;

namespace RobotVacuum.Scripts.Objects;

public partial class Table : StaticBody2D
{
	private Sprite2D _sprite;
	private Area2D _hideRobotArea;

	public override void _Ready()
	{
		_hideRobotArea = GetNode<Area2D>("HideRobotArea");
		_sprite = GetNode<Sprite2D>("Sprite2D");

		_hideRobotArea.BodyEntered += HideRobotArea_OnBodyEntered;
		_hideRobotArea.BodyExited += HideRobotArea_OnBodyExited;
	}

	private void HideRobotArea_OnBodyEntered(Node body)
	{
		if (body is Robot robot)
		{
			robot.CanBeCapturedByEnemy = false;
			SetTableVisibility(false);
		}
	}

	private void HideRobotArea_OnBodyExited(Node body)
	{
		if (body is Robot robot)
		{
			robot.CanBeCapturedByEnemy = true;
			SetTableVisibility(true);
		}
	}

	private void SetTableVisibility(bool visible)
	{
		const float HIDE_ALPHA = 0.5f;
		const float SHOW_ALPHA = 1f;
		const float DURATION = 0.5f;

		float targetAlpha = visible ? SHOW_ALPHA : HIDE_ALPHA;
		Tween tween = CreateTween();
		tween.TweenProperty(_sprite, "modulate:a", targetAlpha, DURATION);
	}
}
