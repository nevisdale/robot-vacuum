using Godot;
using RobotVacuum.Scripts.Audio;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.Objects;

public partial class Charger : Area2D
{
	// do not play sound in outro scene
	[Export]
	private bool _doNotPlaySoundForce = false;


	[Signal]
	public delegate void RoomIsCleanEventHandler();


	// initially robot on the charger.
	// need to avoid playing sound at the start
	private bool _canPlaySound = false;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void RobotEntered()
	{
		if (State.Instance.GarbageLeft == 0)
		{
			if (_canPlaySound)
			{
				AudioManager.Instance.PlaySound_NextLevelOk();
			}
			EmitSignal(SignalName.RoomIsClean);
		}
		else
		{
			if (_canPlaySound)
			{
				AudioManager.Instance.PlaySound_NextLevelError();
			}
		}
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Robot)
		{
			RobotEntered();
		}
	}

	private void OnBodyExited(Node body)
	{
		if (body is Robot)
		{
			if (!_doNotPlaySoundForce)
			{
				_canPlaySound = true;
			}
		}
	}
}
