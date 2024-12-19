using System;
using Godot;

namespace RobotVacuum.Scripts.Garbage;

public partial class PhysicsGarbage : RigidBody2D, IGarbage
{
    private record _dampData(float LinearDamp, float AngularDamp);

    private long _touchedByCarLastTime = 0;

    public long TouchedByRobotLastTime { get; set; }

    private const float TWEEN_DURATION = 0.3f;
    private const float TWEEN_SCALE_FACTOR = 0.2f;

    private CollisionShape2D _collisionShape = null;

    // needed for detecting wet spots and apply damp data
    private int _wetSpotCount = 0;
    private _dampData _initialDampData;
    private readonly _dampData _onWetSpotDampData = new(0f, 0f);

    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _initialDampData = new _dampData(LinearDamp, AngularDamp);
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
        // when you push the physics garbage towards the trash,
        // the robot is pushed out when the the garbage disappears.
        _collisionShape.SetDeferred("disabled", true);
    }

    // Push applies a force to the physics garbage.
    // pass pushForce multiplied by delta to make the force frame rate independent.
    public void Push(KinematicCollision2D kinematicCollision, float pushForce)
    {
        Vector2 force = pushForce * -kinematicCollision.GetNormal();
        Vector2 pos = kinematicCollision.GetPosition() - GlobalPosition;
        ApplyForce(force, pos);
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
        _dampData applyDampData = _onWetSpotDampData;
        if (_wetSpotCount == 0)
        {
            applyDampData = _initialDampData;
        }

        LinearDamp = applyDampData.LinearDamp;
        AngularDamp = applyDampData.AngularDamp;
    }

    public void TouchedByCar()
    {
        _touchedByCarLastTime = DateTime.Now.Ticks;
    }

    public bool IsMovingByCar()
    {
        // must be enough small to be sure that the garbage is moving by car
        long epsilon = TimeSpan.FromSeconds(0.01).Ticks;
        return DateTime.Now.Ticks - _touchedByCarLastTime < epsilon;
    }
}
