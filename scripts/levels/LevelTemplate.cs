using Godot;

namespace Level;

public partial class LevelTemplate : Node2D {
	[Export] private Node2D garbageContainer;
	[Export] private PackedScene nextScene;
	[Export] private Objects.ChargingStation chargingStation;

	public override void _Ready() {
		G.Globals.GarbageCount = garbageContainer.GetChildCount();
		if (chargingStation == null) {
			GD.PrintErr("charging must be not empty");
		}

		RandomRotateGarbage();
	}

	public override void _PhysicsProcess(double delta) {
		G.Globals.GarbageCount = garbageContainer.GetChildCount();

		if (chargingStation.IsComplete()) {
			if (nextScene != null) {
				GetTree().ChangeSceneToPacked(nextScene);
			} else {
				GD.Print("next scene is empty. you complete the level");
			}
		}
	}

	private void RandomRotateGarbage() {
		for (int i = 0; i < garbageContainer.GetChildCount(); i++) {
			Node child = garbageContainer.GetChild(i);
			if (child is Node2D child2d) {
				child2d.Rotate((float)GD.RandRange(0, 2 * Mathf.Pi));
			}
		}
	}
}
