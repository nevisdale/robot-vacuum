using Godot;
using RobotVacuum.Scripts.Audio;

namespace RobotVacuum.Scripts.Levels;

public partial class Outro : Node2D
{
    private const int CAMERA_LIMIT_OFFSET = 64;

    private Camera2D _camera2D = null;
    private DirectionalLight2D _directionalLight2D = null;
    private Robot _robot = null;
    private AudioStreamPlayer _audioStreamPlayer = null;


    private bool _isFullscreen = false;

    public override void _Ready()
    {
        AudioManager.Instance.StopSoundBackground();

        _camera2D = GetNode<Camera2D>("Robot/Camera2D");
        _directionalLight2D = GetNode<DirectionalLight2D>("DirectionalLight2D");
        _robot = GetNode<Robot>("Robot");
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");


        _camera2D.LimitLeft -= CAMERA_LIMIT_OFFSET;
        _camera2D.LimitRight += CAMERA_LIMIT_OFFSET;
        _camera2D.LimitTop -= CAMERA_LIMIT_OFFSET;
        _camera2D.LimitBottom += CAMERA_LIMIT_OFFSET;
        _camera2D.ResetSmoothing();

        Tween tween = CreateTween();
        tween.TweenProperty(_directionalLight2D, "color:a", 1, 15);
        tween.Finished += () =>
        {
            _robot.MakeNotMovable();
            _camera2D.LimitLeft = -1000000;
            _camera2D.LimitRight = 1000000;
            _camera2D.LimitTop = -1000000;
            _camera2D.LimitBottom = 1000000;


            tween = CreateTween();
            tween.TweenProperty(_camera2D, "zoom", new Vector2(1f, 1f), 5);
            tween.SetTrans(Tween.TransitionType.Linear);

            tween.Finished += () =>
            {
                _audioStreamPlayer.Play();
                tween = CreateTween();
                tween.TweenProperty(_camera2D, "zoom", new Vector2(1f, 1f), 9);
                tween.Finished += () =>
                {
                    _robot.InDangerArea();

                    tween = CreateTween();
                    tween.TweenProperty(_camera2D, "zoom", new Vector2(1f, 1f), 4);
                    tween.Finished += () =>
                    {
                        _robot.DisableLight();
                    };
                };
            };
        };

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
