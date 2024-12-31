using Godot;
using RobotVacuum.Scripts.Audio;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.UI;

public partial class PauseMenu : CanvasLayer
{
    [Export(PropertyHint.File)]
    private string _mainMenuScene = default;

    private bool _isPaused = false;

    // game buttons
    private Button _resume = null;
    private Button _restart = null;
    private Button _mainMenu = null;
    private Button _exit = null;

    // music & sound buttons and labels
    private Label _musicLabel = null;
    private Button _musicMore = null;
    private Button _musicLess = null;
    private Label _soundLabel = null;
    private Button _soundMore = null;
    private Button _soundLess = null;

    // other settings
    private Button _windowMode = null;

    public override void _Ready()
    {
        _resume = GetNode<Button>("%Resume");
        _restart = GetNode<Button>("%Restart");
        _mainMenu = GetNode<Button>("%ExitToMainMenu");
        _exit = GetNode<Button>("%ExitToDesktop");

        _musicLabel = GetNode<Label>("%MusicLabel");
        _musicMore = GetNode<Button>("%MusicMore");
        _musicLess = GetNode<Button>("%MusicLess");

        _soundLabel = GetNode<Label>("%SoundLabel");
        _soundMore = GetNode<Button>("%SoundMore");
        _soundLess = GetNode<Button>("%SoundLess");

        _windowMode = GetNode<Button>("%WindowMode");

        _resume.Pressed += () => HidePauseMenu();
        _restart.Pressed += () =>
        {

            SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
            if (gameState.CurrentLevelScene.Contains("intro") || gameState.CurrentLevelScene.Contains("outro"))
            {
                // stop playing intro or outro theme
                AudioManager.Instance.ForceStop();
            }
            HidePauseMenu();
            GoToScene(gameState.CurrentLevelScene);
        };
        _mainMenu.Pressed += () =>
        {
            // there is a little chance to play sound before going to main menu
            AudioManager.Instance.ForceStop();
            HidePauseMenu();
            GoToSceneFast(_mainMenuScene);
        };
        _exit.Pressed += () => TransitionLayer.Instance.Exit();

        _musicMore.Pressed += () => { AudioManager.Instance.MusicVolumeUp(); UpdateVisuals(); };
        _musicLess.Pressed += () => { AudioManager.Instance.MusicVolumeDown(); UpdateVisuals(); };
        _soundMore.Pressed += () => { AudioManager.Instance.SoundVolumeUp(); UpdateVisuals(); };
        _soundLess.Pressed += () => { AudioManager.Instance.SoundVolumeDown(); UpdateVisuals(); };

        _windowMode.Pressed += () => { ToggleWindowMode(); };

        UpdateVisuals();
        HidePauseMenu();
    }

    public void GoToSceneFast(string scenePath)
    {
        // remove all ui because transition layer does not cover it
        GetNode("Control").QueueFree();
        TransitionLayer.Instance.ChangeSceneToFast(scenePath);
    }

    public void GoToScene(string scenePath)
    {
        // remove all ui because transition layer does not cover it
        GetNode("Control").QueueFree();
        TransitionLayer.Instance.ChangeSceneTo(scenePath);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            if (_isPaused)
            {
                HidePauseMenu();
            }
            else
            {
                ShowPauseMenu();
            }
        }
    }

    public void ShowPauseMenu()
    {
        _isPaused = true;
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
        Show();
        UpdateVisuals();
        GetTree().Paused = true;
    }

    public void HidePauseMenu()
    {
        _isPaused = false;
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);
        Hide();
        GetTree().Paused = false;
    }

    public void UpdateVisuals()
    {
        _musicLabel.Text = $"Music: {AudioManager.Instance.MusicVolume}";
        _soundLabel.Text = $"Sound: {AudioManager.Instance.SoundVolume}";

        _windowMode.Text = IsFullscreen() ? "Windowed" : "Fullscreen";
    }

    private void ToggleWindowMode()
    {
        DisplayServer.WindowMode windowMode = DisplayServer.WindowMode.ExclusiveFullscreen;
        if (IsFullscreen())
        {
            windowMode = DisplayServer.WindowMode.Windowed;
        }
        DisplayServer.WindowSetMode(windowMode);
        UpdateVisuals();
    }

    private bool IsFullscreen()
    {
        return DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen;
    }
}
