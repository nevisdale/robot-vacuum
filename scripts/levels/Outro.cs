using Godot;
using RobotVacuum.Scripts.Audio;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.Levels;

public partial class Outro : Node2D
{
    [Export(PropertyHint.File)]
    private string _mainMenuScene = default;

    private const int CAMERA_LIMIT_OFFSET = 64;

    private Camera2D _camera2D = null;
    private DirectionalLight2D _directionalLight2D = null;
    private Robot _robot = null;



    public override void _Ready()
    {
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);

        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        gameState.UpdateCurrentSceneAndAddToAvailable(GetTree());
        SaveManager.Instance.SaveGameState();

        AudioManager.Instance.StopSoundBackgroundSmooth();

        _camera2D = GetNode<Camera2D>("Robot/Camera2D");
        _directionalLight2D = GetNode<DirectionalLight2D>("DirectionalLight2D");
        _robot = GetNode<Robot>("Robot");


        _camera2D.LimitLeft -= CAMERA_LIMIT_OFFSET;
        _camera2D.LimitRight += CAMERA_LIMIT_OFFSET;
        _camera2D.LimitTop -= CAMERA_LIMIT_OFFSET;
        _camera2D.LimitBottom += CAMERA_LIMIT_OFFSET;
        _camera2D.ResetSmoothing();

        // TODO: it looks ugly, animation player is might be better for this
        Tween tween = CreateTween();
        tween.TweenProperty(_directionalLight2D, "color:a", 1, 15);
        tween.Finished += () =>
        {
            // this box can overlap robot lights.
            // red light must be visible in this scene
            GetNode("Env/BoxAngleOpen").QueueFree();

            tween = CreateTween();
            tween.TweenProperty(_camera2D, "zoom", new Vector2(1f, 1f), 5);
            tween.SetTrans(Tween.TransitionType.Linear);

            tween.Finished += () =>
            {
                // intro and outro theme are the same
                AudioStreamPlayer outroTheme = AudioManager.Instance.GetIntroTheme();
                outroTheme.Play();
                outroTheme.Finished += () =>
                {
                    // wait a few seconds
                    Timer timer = new Timer
                    {
                        WaitTime = 3,
                        OneShot = true
                    };
                    AddChild(timer);
                    timer.Start();
                    timer.Timeout += () =>
                    {
                        TransitionLayer.Instance.ChangeSceneTo(_mainMenuScene);
                    };
                };

                tween = CreateTween();
                tween.TweenProperty(_camera2D, "zoom", new Vector2(1f, 1f), 9);
                tween.Finished += () =>
                {
                    _robot.InDangerArea();

                    tween = CreateTween();
                    tween.TweenProperty(_camera2D, "zoom", new Vector2(1f, 1f), 4);
                    tween.Finished += () =>
                    {
                        _robot.MakeNotMovable();
                        _robot.DisableLight();
                    };
                };
            };
        };

    }
}
