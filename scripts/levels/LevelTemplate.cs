using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Objects;

namespace Level;

public partial class LevelTemplate : Node2D {
	private const string RESTART_INPUT_ACTION = "restart";

	[Export] private Node2D garbageContainer;
	[Export] private PackedScene nextScene;
	[Export] private Objects.ChargingStation chargingStation;
	[Export] Robot robot;

	public override void _Ready() {
		G.Globals.GarbageCount = garbageContainer.GetChildCount();

		Debug.Assert(chargingStation != null, "charging must be not empty");
		Debug.Assert(robot != null, "robot must be not empty");

		robot.RobotOnDestroyed += () => {
			Task<Error> task = Global.TransitionLayer.Instance.ReloadCurrentSceneAsync();
			task.ContinueWith(t => {
				if (t.Result != Error.Ok) {
					GD.PrintErr($"can't reload the scene: {t.Result}");
				}
			});
		};

		RandomRotateGarbage();
	}

	public override async void _Process(double delta) {
		if (Input.IsActionJustPressed(RESTART_INPUT_ACTION)) {
			Task<Error> task = Global.TransitionLayer.Instance.ReloadCurrentSceneAsync();
			await task;
			if (task.Result != Error.Ok) {
				GD.PrintErr($"can't reload the scene: {task.Result}");
			}
		}

	}

	public override async void _PhysicsProcess(double delta) {
		G.Globals.GarbageCount = garbageContainer.GetChildCount();

		if (chargingStation.IsComplete()) {
			if (nextScene != null) {
				Task<Error> task = Global.TransitionLayer.Instance.LoadPackedScene(nextScene);
				await task;
				if (task.Result != Error.Ok) {
					GD.PrintErr($"can't load the next scene: {task.Result}");
				}

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
