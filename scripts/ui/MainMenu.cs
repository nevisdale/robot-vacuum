using Godot;
using RobotVacuum.Scripts.Audio;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.UI;

public partial class MainMenu : CanvasLayer
{
    [Export(PropertyHint.File)]
    private string _startScene = default;

    [Export(PropertyHint.File)]
    private string _selectScene = default;

    private Button _play = null;
    private Button _continue = null;
    private Button _select = null;
    private Button _windowMode = null;
    private Button _exit = null;

    public override void _Ready()
    {
        AudioManager.Instance.StopSoundBackground();
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);

        _play = GetNode<Button>("%Play");
        _continue = GetNode<Button>("%Continue");
        _select = GetNode<Button>("%Select");
        _windowMode = GetNode<Button>("%WindowMode");
        _exit = GetNode<Button>("%Exit");

        _play.Pressed += () => { GoToScene(_startScene); };
        _select.Pressed += () => { GoToSceneFast(_selectScene); };
        _windowMode.Pressed += ToggleWindowMode;
        _exit.Pressed += () => TransitionLayer.Instance.Exit();

        UpdateVisual();
        TryGetSavedData();
    }

    private void TryGetSavedData()
    {
        string currentScenePath = SaveManager.Instance.GetGameState().CurrentLevelScene;
        if (currentScenePath == null || currentScenePath.Length == 0)
        {
            _continue.Disabled = true;
            return;
        }

        _continue.Disabled = false;
        _continue.Pressed += () =>
        {
            GoToScene(currentScenePath);
        };
    }

    private void GoToScene(string scenePath)
    {
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);
        // remove all ui because transition layer does not cover it
        GetNode("Control").QueueFree();
        TransitionLayer.Instance.ChangeSceneTo(scenePath);
    }

    private void GoToSceneFast(string scenePath)
    {
        TransitionLayer.Instance.ChangeSceneToFast(scenePath);
    }

    private void ToggleWindowMode()
    {
        DisplayServer.WindowMode windowMode = DisplayServer.WindowMode.ExclusiveFullscreen;
        if (IsFullscreen())
        {
            windowMode = DisplayServer.WindowMode.Windowed;
        }
        DisplayServer.WindowSetMode(windowMode);
        UpdateVisual();
    }

    private bool IsFullscreen()
    {
        return DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen;
    }

    private void UpdateVisual()
    {
        if (IsFullscreen())
        {
            _windowMode.Text = "Windowed";
        }
        else
        {
            _windowMode.Text = "Fullscreen";
        }
    }
}
