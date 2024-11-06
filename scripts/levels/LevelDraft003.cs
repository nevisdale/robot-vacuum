using Godot;
using RobotVacuum.Scripts.Enemies;
using RobotVacuum.Scripts.Objects;

namespace RobotVacuum.Scripts.Levels;

public partial class LevelDraft003 : LevelTemplate
{
    private Car _car = null;
    private FloorButton _carGoToLeftButton = null;
    private FloorButton _carGoToRightButton = null;

    public override void _Ready()
    {
        base._Ready();

        _car = GetNode<Car>("Enemies/Car");
        _carGoToLeftButton = GetNode<FloorButton>("CarGoToLeftButton");
        _carGoToRightButton = GetNode<FloorButton>("CarGoToRightButton");

        _carGoToLeftButton.ButtonPressed += () => _car.SetDirectionTo(Vector2.Left);
        _carGoToRightButton.ButtonPressed += () => _car.SetDirectionTo(Vector2.Right);
    }
}
