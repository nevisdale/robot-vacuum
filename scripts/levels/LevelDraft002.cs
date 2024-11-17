using Godot;
using RobotVacuum.Scripts.Enemies;
using RobotVacuum.Scripts.Objects;

namespace RobotVacuum.Scripts.Levels;

public partial class LevelDraft002 : LevelTemplate
{
    private Car _car1 = null;
    private Car _car2 = null;

    private FloorButton _carGoToLeftButton = null;
    private FloorButton _carGoToRightButton = null;

    public override void _Ready()
    {
        base._Ready();

        _car1 = GetNode<Car>("Enemies/Car");
        _car2 = GetNode<Car>("Enemies/Car2");

        _carGoToLeftButton = GetNode<FloorButton>("CarGoToLeftButton");
        _carGoToRightButton = GetNode<FloorButton>("CarGoToRightButton");

        _carGoToLeftButton.ButtonPressed += OnCarGoToLeftButtonPressed;
        _carGoToRightButton.ButtonPressed += OnCarGoToRightButtonPressed;
    }

    private void OnCarGoToLeftButtonPressed()
    {
        _car1.SetDirectionTo(Vector2.Left);
        _car2.SetDirectionTo(Vector2.Left);
    }

    private void OnCarGoToRightButtonPressed()
    {
        _car1.SetDirectionTo(Vector2.Right);
        _car2.SetDirectionTo(Vector2.Right);
    }
}
