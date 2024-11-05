using Godot;
using RobotVacuum.Scripts.Globals;
using RobotVacuum.Scripts.Objects;

namespace RobotVacuum.Scripts.Levels;

public partial class LevelTemplate : Node2D
{
	[Export]
	private PackedScene _nextLevelScene = null;

	private Robot _robot = null;
	private Node2D _garbageContainer = null;
	private Charger _charger = null;

	public override void _Ready()
	{
		_robot = GetNode<Robot>("Robot");
		_garbageContainer = GetNode<Node2D>("Garbage");
		_charger = GetNodeOrNull<Charger>("Charger");

		State.Instance.GarbageLeft = _garbageContainer.GetChildCount();
		_robot.CapturedByEnemy += Robot_OnCapturedByEnemy;
		_charger.RoomIsClean += Charger_OnRoomIsClean;
	}

	public override void _Process(double delta)
	{
		State.Instance.GarbageLeft = _garbageContainer.GetChildCount();
		if (Input.IsActionJustPressed("restart"))
		{
			TransitionLayer.Instance.ReloadCurrentScene();
		}
	}

	private void Robot_OnCapturedByEnemy()
	{
		TransitionLayer.Instance.ReloadCurrentScene();
	}

	private void Charger_OnRoomIsClean()
	{
		GD.Print("level completed");
		TransitionLayer.Instance.ChangeSceneTo(_nextLevelScene);
	}
}
