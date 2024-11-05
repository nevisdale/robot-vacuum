using Godot;

namespace RobotVacuum.Scripts.Garbage;

public partial class Seed : Area2D, IGarbage
{
    private const float TWEEN_DURATION = 0.3f;
    private const float TWEEN_SCALE_FACTOR = 0.2f;

    public override void _Ready() => Rotate(GD.Randf() * Mathf.Pi * 2f);

    public bool CanBeCapturedByBin() => false;
    public bool CanBeCapturedByRobot() => true;

    public void Capture(Node2D capturer)
    {
        Tween tween = CreateTween();
        tween.SetParallel(true);
        tween.Finished += QueueFree;
        tween.TweenProperty(this, "global_position", capturer.GlobalPosition, TWEEN_DURATION);
        tween.TweenProperty(this, "scale", Scale * TWEEN_SCALE_FACTOR, TWEEN_DURATION);
    }
}
