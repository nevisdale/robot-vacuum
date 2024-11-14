using Godot;
using RobotVacuum.Scripts.Enemies;
using RobotVacuum.Scripts.Objects;

namespace RobotVacuum.Scripts.Levels;

public partial class Level6 : LevelTemplate
{
    private Car _car = null;
    private FloorButton _carGoToRightButton = null;

    public override void _Ready()
    {
        base._Ready();

        _car = GetNode<Car>("Enemies/Car");
        _carGoToRightButton = GetNode<FloorButton>("CarGoToRightButton");

        _carGoToRightButton.ButtonPressed += () => _car.SetDirectionTo(Vector2.Right);
    }
}
