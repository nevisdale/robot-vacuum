using Godot;
using System;

namespace Objects;

public partial class ChargingStation : Area2D {
	private bool isComplete = false;

	public override void _Ready() {
		BodyEntered += Area2D_OnBodyEntered;
	}

	private void Area2D_OnBodyEntered(Node body) {
		if (G.Globals.GarbageCount == 0) {
			isComplete = true;
		}
	}

	public bool IsComplete() {
		return isComplete;
	}
}
