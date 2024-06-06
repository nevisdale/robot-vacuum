using Godot;
using System;

namespace Objects;

public partial class Bin : StaticBody2D {
	private Area2D CaptureArea;

	public override void _Ready() {
		CaptureArea = GetNode<Area2D>("GarbageCaptureArea");

		CaptureArea.BodyEntered += CaptureArea_OnBodyEntered;
	}


	private void CaptureArea_OnBodyEntered(Node node) {
		if (node is not Garbage.IGarbage garbage) {
			return;
		}

		if (garbage.CanBeCapturedByBin()) {
			garbage.CaptureAndDestroy(GlobalPosition);
		}
	}
}
