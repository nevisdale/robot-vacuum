using System.Collections.Generic;
using Godot;
using RobotVacuum.Scripts.Enemies;
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
	private Node2D _leftButtonsGroup = null;
	private Node2D _rightButtonsGroup = null;
	private Node2D _enemyGroup = null;
	private List<Car> _cars = new();

	public override void _Ready()
	{
		_robot = GetNode<Robot>("Robot");
		_garbageContainer = GetNode<Node2D>("Garbage");
		_charger = GetNodeOrNull<Charger>("Charger");
		_leftButtonsGroup = GetNode<Node2D>("Buttons/Left");
		_rightButtonsGroup = GetNode<Node2D>("Buttons/Right");
		_enemyGroup = GetNode<Node2D>("Enemies");

		List<FloorButton> leftButtons = GetOnlyTypedChildrenFrom<FloorButton>(_leftButtonsGroup);
		List<FloorButton> rightButtons = GetOnlyTypedChildrenFrom<FloorButton>(_rightButtonsGroup);
		_cars = GetOnlyTypedChildrenFrom<Car>(_enemyGroup);
		foreach (FloorButton button in leftButtons)
		{
			button.ButtonPressed += () => MoveAllCarsTo(Vector2.Left);
		}
		foreach (FloorButton button in rightButtons)
		{
			button.ButtonPressed += () => MoveAllCarsTo(Vector2.Right);
		}

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

	private void MoveAllCarsTo(Vector2 direction)
	{
		foreach (Car car in _cars)
		{
			car.SetDirectionTo(direction);
		}
	}

	private static List<T> GetOnlyTypedChildrenFrom<T>(Node2D group) where T : Node
	{
		List<T> children = new();
		foreach (Node node in group.GetChildren())
		{
			if (node is T child)
			{
				children.Add(child);
			}
		}
		return children;
	}
}
