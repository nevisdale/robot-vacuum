using Godot;
using System;
using System.Threading.Tasks;

namespace Global;

public partial class TransitionLayer : CanvasLayer {
	const string BLACK_SCREEN_ANIMATION = "black_screen";

	public static TransitionLayer Instance { get; private set; }


	[Export] AnimationPlayer animationPlayer;
	[Export] ColorRect colorRect;

	public override void _Ready() {
		Instance = this;
		colorRect.SelfModulate = new Color(1, 1, 1, 0);
	}

	public async Task<Error> ReloadCurrentSceneAsync() {
		animationPlayer.Play(BLACK_SCREEN_ANIMATION);
		await ToSignal(animationPlayer, AnimationMixer.SignalName.AnimationFinished);

		animationPlayer.CallDeferred(AnimationPlayer.MethodName.PlayBackwards, BLACK_SCREEN_ANIMATION);
		return GetTree().ReloadCurrentScene();
	}

	public async Task<Error> LoadPackedScene(PackedScene packedScene) {
		animationPlayer.Play(BLACK_SCREEN_ANIMATION);
		await ToSignal(animationPlayer, AnimationMixer.SignalName.AnimationFinished);

		animationPlayer.CallDeferred(AnimationPlayer.MethodName.PlayBackwards, BLACK_SCREEN_ANIMATION);
		return GetTree().ChangeSceneToPacked(packedScene);
	}
}
