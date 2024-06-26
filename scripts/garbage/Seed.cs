using Godot;

namespace Garbage;

public partial class Seed : Area2D, IGarbage {
	const float SCALE_FACTOR = 0.2f;
	const float TWEEN_DURATION = 0.3f;

	public bool CanBeCapturedByBin() {
		return false;
	}

	public bool CanBeCapturedByRobot() {
		return true;
	}

	public void CaptureAndDestroy(Vector2 capturerGlobalPosition) {
		Tween tween = CreateTween().SetParallel(true);
		tween.TweenProperty(this, Node2D.PropertyName.GlobalPosition.ToString(), capturerGlobalPosition, TWEEN_DURATION);
		tween.TweenProperty(this, Node2D.PropertyName.Scale.ToString(), Scale * SCALE_FACTOR, TWEEN_DURATION);

		tween.Finished += QueueFree;
		tween.Play();
	}
}
