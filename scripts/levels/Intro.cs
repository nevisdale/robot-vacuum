using Godot;
using RobotVacuum.Scripts.Audio;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.Levels;

public partial class Intro : Node2D
{
    [Export(PropertyHint.File)]
    private string _nextLevelScene = null;

    private Robot _robot = null;
    private Timer _startAudioTimer = null;
    private ColorRect _blackColorRect = null;
    private Camera2D _camera2D = null;


    public override async void _Ready()
    {
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);

        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        gameState.UpdateCurrentSceneAndAddToAvailable(GetTree());
        SaveManager.Instance.SaveGameState();

        _robot = GetNode<Robot>("Robot");
        _startAudioTimer = GetNode<Timer>("StartAudioTimer");
        _blackColorRect = GetNode<ColorRect>("BlackColorRect");
        _camera2D = GetNode<Camera2D>("Robot/Camera2D");

        AudioStreamPlayer introTheme = AudioManager.Instance.GetIntroTheme();

        _robot.MakeNotMovable();

        _startAudioTimer.Start();
        await ToSignal(_startAudioTimer, "timeout");

        introTheme.Play();
        await ToSignal(introTheme, "finished");

        Tween tween = CreateTween();
        tween.TweenProperty(_blackColorRect, "modulate:a", 0, 1);
        tween.TweenProperty(_camera2D, "zoom", new Vector2(1f, 1f), 2);
        tween.TweenProperty(_camera2D, "zoom", new Vector2(0.5f, 0.5f), 2);
        await ToSignal(tween, "finished");

        TransitionLayer.Instance.ChangeSceneTo(_nextLevelScene);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("restart"))
        {
            AudioManager.Instance.GetIntroTheme().Stop();
            TransitionLayer.Instance.ReloadCurrentScene();
        }
    }
}
