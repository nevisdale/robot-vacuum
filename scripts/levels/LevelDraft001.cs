using Godot;
using RobotVacuum.Scripts.Objects;
using RobotVacuum.Scripts.Enemies;

namespace RobotVacuum.Scripts.Levels;

public partial class LevelDraft001 : LevelTemplate
{
	private FloorButton _carGoToLeftButton = null;
	private FloorButton _carGoToRightButton = null;
	private Car _car = null;

	public override void _Ready()
	{
		base._Ready();

		_carGoToLeftButton = GetNode<FloorButton>("CarGoToLeftButton");
		_carGoToRightButton = GetNode<FloorButton>("CarGoToRightButton");
		_car = GetNode<Car>("Enemies/Car");

		_carGoToLeftButton.ButtonPressed += CarGoToLeftButton_OnButtonPressed;
		_carGoToRightButton.ButtonPressed += CarGoToRightButton_OnButtonPressed;
	}

	private void CarGoToLeftButton_OnButtonPressed() => _car.SetDirectionTo(Vector2.Left);
	private void CarGoToRightButton_OnButtonPressed() => _car.SetDirectionTo(Vector2.Right);
}
