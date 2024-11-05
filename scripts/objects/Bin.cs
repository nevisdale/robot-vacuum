using Godot;
using RobotVacuum.Scripts.Garbage;

namespace RobotVacuum.Scripts.Objects;

public partial class Bin : StaticBody2D
{
    private Area2D _captureArea = null;

    public override void _Ready()
    {
        _captureArea = GetNode<Area2D>("GarbageCaptureArea");
        _captureArea.BodyEntered += CaptureArea_OnBodyEntered;
    }

    private bool TryCaptureGarbage(IGarbage garbage)
    {
        if (garbage.CanBeCapturedByBin())
        {
            garbage.Capture(this);
            return true;
        }
        return false;
    }

    private void CaptureArea_OnBodyEntered(Node2D body)
    {
        bool isGarbage = false;
        bool captured = false;

        if (body is IGarbage garbage)
        {
            isGarbage = true;
            captured = TryCaptureGarbage(garbage);
        }

        GD.Print($"{Name}. CaptureArea_OnBodyEntered: body={body.Name}, IsGarbage={isGarbage}, Captured={captured}");
    }
}
