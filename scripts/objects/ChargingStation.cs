using Godot;
using System;

namespace Objects;

public partial class ChargingStation : Area2D {
	public override void _Ready() {
		BodyEntered += Area2D_OnBodyEntered;
	}

	private void Area2D_OnBodyEntered(Node body) {
		if (G.Globals.GarbageCount == 0) {
			GD.Print("you are cool :)");
		}
	}
}
