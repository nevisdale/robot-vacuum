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
        if (body is PhysicsGarbage garbage)
        {
            GD.Print($"{garbage.Name} entered wet spot {Name}");
            garbage.AddWetSpot();
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is PhysicsGarbage garbage)
        {
            GD.Print($"{garbage.Name} removed from wet spot {Name}");
            garbage.RemoveWetSpot();
        }
    }
}
