using Godot;

namespace RobotVacuum.Scripts.Garbage;

public partial class Can : RigidBody2D, IGarbage
{
    private record _CanDampData(float LinearDamp, float AngularDamp);

    private const float TWEEN_DURATION = 0.3f;
    private const float TWEEN_SCALE_FACTOR = 0.2f;

    private CollisionShape2D _collisionShape = null;

    // needed for detecting wet spots and apply damp data
    private int _wetSpotCount = 0;
    private _CanDampData _initialDampData;
    private readonly _CanDampData _onWetSpotDampData = new(0f, 0f);

    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _initialDampData = new _CanDampData(LinearDamp, AngularDamp);
    }

    public bool CanBeCapturedByBin() => true;
    public bool CanBeCapturedByRobot() => false;

    public void Capture(Node2D capturer)
    {
        Tween tween = CreateTween();
        tween.SetParallel(true);
        tween.Finished += QueueFree;
        tween.TweenProperty(this, "global_position", capturer.GlobalPosition, TWEEN_DURATION);
        tween.TweenProperty(this, "scale", Scale * TWEEN_SCALE_FACTOR, TWEEN_DURATION);

        // strange behavior:
        // when you push the can towards the trash can,
        // the robot is pushed out when the can disappears.
        _collisionShape.SetDeferred("disabled", true);
    }

    public void AddWetSpot()
    {
        _wetSpotCount++;
        UpdateDamp();
    }

    public void RemoveWetSpot()
    {
        _wetSpotCount--;
        UpdateDamp();
    }

    private void UpdateDamp()
    {
        _CanDampData applyDampData = _onWetSpotDampData;
        if (_wetSpotCount == 0)
        {
            applyDampData = _initialDampData;
        }

        LinearDamp = applyDampData.LinearDamp;
        AngularDamp = applyDampData.AngularDamp;
    }
}
