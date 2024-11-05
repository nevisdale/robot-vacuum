using Godot;

namespace RobotVacuum.Scripts.Globals;

public partial class TransitionLayer : CanvasLayer
{
    private const string ANIM_NAME = "black_screen";

    public static TransitionLayer Instance { get; private set; }

    private AnimationPlayer _animationPlayer = null;
    private ColorRect _blackScreen = null;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _blackScreen = GetNode<ColorRect>("ColorRect");

        _blackScreen.SelfModulate = new Color(1, 1, 1, 0);

        Instance = this;
    }

    public async void ReloadCurrentScene()
    {
        _animationPlayer.Play(ANIM_NAME);
        await ToSignal(_animationPlayer, "animation_finished");
        _animationPlayer.CallDeferred("play_backwards", ANIM_NAME);

        Node currentScene = GetTree().CurrentScene;

        if (currentScene == null)
        {
            GD.PrintErr($"{Name}. current scene is null. do nothing");
            return;
        }

        Error err = GetTree().ReloadCurrentScene();
        if (err != Error.Ok)
        {
            GD.PrintErr($"{Name}. reloading scene {currentScene.Name}: {err}");
            return;
        }
        GD.Print($"{Name}. reloaded scene {currentScene.Name}");
    }

    public async void ChangeSceneTo(PackedScene scene)
    {
        if (scene == null)
        {
            GD.PrintErr($"{Name}. scene is null. do nothing");
            return;
        }

        _animationPlayer.Play(ANIM_NAME);
        await ToSignal(_animationPlayer, "animation_finished");
        _animationPlayer.CallDeferred("play_backwards", ANIM_NAME);

        Error err = GetTree().ChangeSceneToPacked(scene);
        if (err != Error.Ok)
        {
            GD.PrintErr($"{Name}. changing scene: {err}");
            return;
        }

        GD.Print($"{Name}. changed scene to {scene.ResourcePath}");
    }
}
