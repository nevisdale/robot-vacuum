using Godot;
using System;

public partial class Seed3 : Area2D {
	const float SCALE_FACTOR = 0.2f;
	const float TWEEN_DURATION = 0.3f;

	public override void _Ready() {
		AreaEntered += OnArea2DAreaEntered;
	}

	private void OnArea2DAreaEntered(Area2D area) {
		Tween tween = CreateTween().SetParallel(true);
		tween.TweenProperty(this, "global_position", area.GlobalPosition, TWEEN_DURATION);
		tween.TweenProperty(this, "scale", Scale * SCALE_FACTOR, TWEEN_DURATION);

		tween.Finished += () => { QueueFree(); };
		tween.Play();
	}
}
