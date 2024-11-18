using Godot;

namespace RobotVacuum.Scripts.Objects;

public partial class Table : StaticBody2D
{
	private Sprite2D _sprite;
	private Area2D _hideRobotArea;
	private ColorRect _colorRect;

	public override void _Ready()
	{
		_hideRobotArea = GetNode<Area2D>("HideRobotArea");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_colorRect = GetNode<ColorRect>("ColorRect");

		// Visible my be disabled in the editor,
		// because it prevents the editor choose other nodes
		_colorRect.Visible = true;

		_hideRobotArea.BodyEntered += HideRobotArea_OnBodyEntered;
		_hideRobotArea.BodyExited += HideRobotArea_OnBodyExited;
	}

	private void HideRobotArea_OnBodyEntered(Node body)
	{
		if (body is Robot robot)
		{
			robot.CanBeCapturedByEnemy = false;
			robot.OutDanderAreaForce();
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
		const float DURATION = 0.5f;

		float colorRectAlpha = visible ? 0 : 0.2f;
		float spriteAlpha = visible ? 1f : 0.5f;

		Tween tween = CreateTween();
		tween.SetParallel(true);
		tween.TweenProperty(_sprite, "modulate:a", spriteAlpha, DURATION);
		tween.TweenProperty(_colorRect, "color:a", colorRectAlpha, DURATION);
	}
}
