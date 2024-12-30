using Godot;
using RobotVacuum.Scripts.Audio;

namespace RobotVacuum.Scripts.Objects;

public partial class FloorButton : Area2D
{
	[Signal] public delegate void ButtonPressedEventHandler();
	[Signal] public delegate void ButtonReleasedEventHandler();

	// several obejcts can press the button at the same time
	// so we need to keep track of how many objects are pressing the button
	// to be sure that the button is pressed and released correctly
	private int _pressedCount = 0;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node body)
	{
		_pressedCount += 1;
		if (_pressedCount == 1)
		{
			GD.Print($"{Name} pressed by {body.Name}");
			EmitSignal(SignalName.ButtonPressed);
			AudioManager.Instance.PlaySound_ActivateButton();
		}
	}

	private void OnBodyExited(Node body)
	{
		_pressedCount -= 1;
		if (_pressedCount == 0)
		{
			GD.Print($"{Name} released by {body.Name}");
			EmitSignal(SignalName.ButtonReleased);
			AudioManager.Instance.PlaySound_ActivateButton();
		}
	}
}
