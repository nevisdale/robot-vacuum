using Godot;

namespace RobotVacuum.Scripts.Garbage;

public partial class CollectableGarbage : Area2D, IGarbage
{
    private const float TWEEN_DURATION = 0.3f;
    private const float TWEEN_SCALE_FACTOR = 0.2f;

    public override void _Ready() => Rotate(GD.Randf() * Mathf.Pi * 2f);

    public bool CanBeCapturedByBin() => false;
    public bool CanBeCapturedByRobot() => true;

    private Node2D _capturer;

    public override void _Process(double delta)
    {
        if (_capturer != null)
        {
            // do not use in tween because global position might be changed
            GlobalPosition = GlobalPosition.Lerp(_capturer.GlobalPosition, (float)delta * 5);
        }
    }

    public void Capture(Node2D capturer)
    {
        _capturer = capturer;

        Tween tween = CreateTween();
        tween.SetParallel(true);
        tween.Finished += QueueFree;
        tween.TweenProperty(this, "scale", Scale * TWEEN_SCALE_FACTOR, TWEEN_DURATION);
    }
}
