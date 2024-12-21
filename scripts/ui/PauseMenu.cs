using Godot;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.UI;

public partial class PauseMenu : CanvasLayer
{
    [Export(PropertyHint.File)]
    private string _mainMenuScene = default;

    private bool _isPaused = false;

    private Button _resume = null;
    private Button _mainMenu = null;
    private Button _exit = null;

    public override void _Ready()
    {
        _resume = GetNode<Button>("%Resume");
        _mainMenu = GetNode<Button>("%ExitToMainMenu");
        _exit = GetNode<Button>("%ExitToDesktop");

        _resume.Pressed += () => HidePauseMenu();
        _mainMenu.Pressed += () =>
        {
            HidePauseMenu();
            GoToScene(_mainMenuScene);
        };
        _exit.Pressed += () => GetTree().Quit();

        HidePauseMenu();
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
        GetTree().Paused = true;
    }

    public void HidePauseMenu()
    {
        _isPaused = false;
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);
        Hide();
        GetTree().Paused = false;
    }
}
