using Godot;

namespace Level;

public partial class LevelTemplate : Node2D {
	[Export]
	private Node2D garbageContainer;

	public override void _Ready() {
		G.Globals.GarbageCount = garbageContainer.GetChildCount();
	}

	public override void _PhysicsProcess(double delta) {
		G.Globals.GarbageCount = garbageContainer.GetChildCount();
	}
}
