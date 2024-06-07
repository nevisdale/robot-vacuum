using Godot;

namespace UI;

public partial class UI : CanvasLayer {
	[Export]
	private Label label;

	public override void _Ready() {
		if (label == null) {
			GD.PrintErr("label must be set");
		}
	}

	public override void _Process(double delta) {
		string labelText = "go to charging station";
		if (!IsComplete()) {
			labelText = $"you need to capture {G.Globals.GarbageCount} garbage items";
		}
		label.Text = labelText;
	}

	public bool IsComplete() {
		return G.Globals.GarbageCount == 0;
	}
}
