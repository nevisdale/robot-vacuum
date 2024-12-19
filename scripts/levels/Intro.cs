using Godot;
using RobotVacuum.Scripts;
using RobotVacuum.Scripts.Globals;
using System;
using System.Reflection.Metadata;

namespace RobotVacuum.Scripts.Levels;

public partial class Intro : Node2D
{
    [Export(PropertyHint.File)]
    private string _nextLevelScene = null;

    private AudioStreamPlayer _audioStreamPlayer;
    private Robot _robot = null;
    private Timer _startAudioTimer = null;
    private ColorRect _blackColorRect = null;
    private Camera2D _camera2D = null;

    private bool _isFullscreen = false;


    public override void _Ready()
    {
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        _robot = GetNode<Robot>("Robot");
        _startAudioTimer = GetNode<Timer>("StartAudioTimer");
        _blackColorRect = GetNode<ColorRect>("BlackColorRect");
        _camera2D = GetNode<Camera2D>("Robot/Camera2D");

        _robot.MakeNotMovable();
        _startAudioTimer.Timeout += () =>
        {
            _audioStreamPlayer.Finished += () =>
            {
                Tween tween = CreateTween();
                tween.TweenProperty(_blackColorRect, "modulate:a", 0, 1);
                tween.TweenProperty(_camera2D, "zoom", new Vector2(1f, 1f), 2);
                tween.TweenProperty(_camera2D, "zoom", new Vector2(0.5f, 0.5f), 2);
                tween.Finished += () =>
                {
                    TransitionLayer.Instance.ChangeSceneTo(_nextLevelScene);
                };
            };

            _audioStreamPlayer.Play();
        };
        _startAudioTimer.Start();


    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("fullscreen"))
        {
            ToggleWindowMode();
        }
    }

    private void ToggleWindowMode()
    {
        DisplayServer.WindowMode windowMode = DisplayServer.WindowMode.Fullscreen;
        DisplayServer.MouseMode mouseMode = DisplayServer.MouseMode.Hidden;
        if (_isFullscreen)
        {
            windowMode = DisplayServer.WindowMode.Windowed;
            mouseMode = DisplayServer.MouseMode.Visible;
            _isFullscreen = false;
        }
        else
        {
            _isFullscreen = true;
        }
        DisplayServer.WindowSetMode(windowMode);
        DisplayServer.MouseSetMode(mouseMode);
    }
}
