using Godot;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.UI;

public partial class MainMenu : CanvasLayer
{
    [Export(PropertyHint.File)]
    private string _startScene = default;

    private Button _play = null;
    private Button _continue = null;
    private Button _exit = null;

    public override void _Ready()
    {
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);

        _play = GetNode<Button>("%Play");
        _continue = GetNode<Button>("%Continue");
        _exit = GetNode<Button>("%Exit");

        _play.Pressed += () => { GoToScene(_startScene); };
        _exit.Pressed += () => GetTree().Quit();

        TryGetSavedData();
    }

    private void TryGetSavedData()
    {
        if (!SaveManager.CanLoadGame())
        {
            _continue.Disabled = true;
            return;
        }

        _continue.Disabled = false;
        _continue.Pressed += () =>
        {
            string scenePath = SaveManager.GetScenePathToLoad();
            GoToScene(scenePath);
        };
    }

    private void GoToScene(string scenePath)
    {
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);
        // remove all ui because transition layer does not cover it
        GetNode("Control").QueueFree();
        TransitionLayer.Instance.ChangeSceneTo(scenePath);
    }
}
