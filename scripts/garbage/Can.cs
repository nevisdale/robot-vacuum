using Godot;

namespace Garbage;

public partial class Can : RigidBody2D, IGarbage {
	const float SCALE_FACTOR = 0.2f;
	const float TWEEN_DURATION = 0.3f;

	CollisionShape2D collisionShape;

	public override void _Ready() {
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public bool CanBeCapturedByBin() {
		return true;
	}

	public bool CanBeCapturedByRobot() {
		return false;
	}

	public void CaptureAndDestroy(Vector2 capturerGlobalPosition) {

		Tween tween = CreateTween().SetParallel(true);
		tween.TweenProperty(this, Node2D.PropertyName.GlobalPosition.ToString(), capturerGlobalPosition, TWEEN_DURATION);
		tween.TweenProperty(this, Node2D.PropertyName.Scale.ToString(), Scale * SCALE_FACTOR, TWEEN_DURATION);

		tween.Finished += QueueFree;
		tween.Play();

		// странный эфект.
		// когда толкаешь банку в сторону мусорки, то робота выталкивает, когда банка изчезает.
		collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
}
