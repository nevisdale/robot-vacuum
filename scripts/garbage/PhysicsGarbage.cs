using System;
using System.Diagnostics;
using Godot;

namespace RobotVacuum.Scripts.Garbage;

public partial class PhysicsGarbage : RigidBody2D, IGarbage, IElectricityReceiver
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

    // prevent sending electricity 2 times from the same garbage pair
    private long _hasElectricityLastTime = 0;
    private bool _hasElectricity = false;
    public bool HasElectricity
    {
        get => _hasElectricity;
        set
        {
            _hasElectricity = value;
            if (_hasElectricity)
            {
                _hasElectricityLastTime = DateTime.Now.Ticks;
            }
            ApplyElectricityVisual();
        }
    }

    private string _name;

    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _initialDampData = new _dampData(LinearDamp, AngularDamp);

        _name = GetParent().Name;

        BodyEntered += PhysicsGarbage_BodyEntered;
    }

    private void PhysicsGarbage_BodyEntered(Node body)
    {
        IElectricityReceiver receiver = body as IElectricityReceiver;
        if (receiver != null)
        {
            SendElectricityIfPossible(receiver);
        }
    }

    public bool CanBeCapturedByBin() => true;
    public bool CanBeCapturedByRobot() => false;

    public void Capture(Node2D capturer)
    {
        Tween tween = CreateTween();
        tween.SetParallel(true);
        tween.Finished += () =>
        {
            Node parent = GetParent();
            Debug.Assert(parent != null, $"{Name} must have a parent");
            parent.QueueFree();
        };
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

    public bool SendElectricityIfPossible(IElectricityReceiver receiver)
    {
        long electricityDelay = TimeSpan.FromSeconds(0.05).Ticks;
        bool canSendElectricity = DateTime.Now.Ticks - _hasElectricityLastTime >= electricityDelay;

        if (HasElectricity && canSendElectricity)
        {
            receiver.ReceiveElectricity();
            HasElectricity = false;
            return true;
        }
        return false;
    }

    public void ReceiveElectricity()
    {
        HasElectricity = true;
    }

    private void ApplyElectricityVisual()
    {
        if (_hasElectricity)
        {
            Modulate = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            Modulate = new Color(1, 1, 1, 1);
        }
    }
}
