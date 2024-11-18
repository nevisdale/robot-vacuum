using Godot;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.Objects;

public partial class Charger : Area2D
{
	[Signal] public delegate void RoomIsCleanEventHandler();

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void RobotEntered()
	{
		if (State.Instance.GarbageLeft == 0)
		{
			EmitSignal(SignalName.RoomIsClean);
		}
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Robot)
		{
			RobotEntered();
		}
	}
}
