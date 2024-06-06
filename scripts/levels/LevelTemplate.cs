using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;

public partial class LevelTemplate : Node2D {
	private Godot.Collections.Array<PackedScene> garbageScenes = new(){
		ResourceLoader.Load<PackedScene>("res://scenes/garbage/seed_1.tscn"),
		ResourceLoader.Load<PackedScene>("res://scenes/garbage/seed_2.tscn"),
		ResourceLoader.Load<PackedScene>("res://scenes/garbage/seed_3.tscn"),
	};

	private Node2D garbageContainer;

	public override void _Ready() {
		garbageContainer = GetNode<Node2D>("GarbageContainer");

		CreateGarbageN(300);
	}

	private void CreateGarbageN(int count) {
		Vector2 viewPortSize = GetViewportRect().Size;
		for (int i = 0; i < count; i++) {
			Vector2 position = new(
				 (float)GD.RandRange(0, viewPortSize.X),
				 (float)GD.RandRange(0, viewPortSize.Y)
			);

			Node2D garbageInstance = garbageScenes.PickRandom().Instantiate<Garbage.Seed>();
			garbageInstance.Position = position;
			garbageInstance.Rotate((float)GD.RandRange(0, 2 * Mathf.Pi));

			garbageContainer.AddChild(garbageInstance);
		}
	}
}
