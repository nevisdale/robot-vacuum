using Godot;
using RobotVacuum.Scripts.Garbage;

namespace RobotVacuum.Scripts.Objects;

public partial class Electricity : Area2D
{
    public override void _Ready()
    {

        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        IElectricityReceiver receiver = body as IElectricityReceiver;
        receiver?.ReceiveElectricity();
    }
}
