using Godot;
using RobotVacuum.Scripts.Garbage;

namespace RobotVacuum.Scripts.Objects;

public partial class WetSpot : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Can can)
        {
            GD.Print($"{can.Name} entered wet spot {Name}");
            can.AddWetSpot();
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is Can can)
        {
            GD.Print($"{can.Name} removed from wet spot {Name}");
            can.RemoveWetSpot();
        }
    }
}
