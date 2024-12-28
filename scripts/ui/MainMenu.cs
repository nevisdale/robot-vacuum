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
    private Button _exit = null;

    public override void _Ready()
    {
        AudioManager.Instance.StopSoundBackground();
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);

        _play = GetNode<Button>("%Play");
        _continue = GetNode<Button>("%Continue");
        _select = GetNode<Button>("%Select");
        _exit = GetNode<Button>("%Exit");

        _play.Pressed += () => { GoToScene(_startScene); };
        _select.Pressed += () => { GoToSceneFast(_selectScene); };
        _exit.Pressed += () => TransitionLayer.Instance.Exit();

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
        // DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);
        TransitionLayer.Instance.ChangeSceneToFast(scenePath);
    }
}
