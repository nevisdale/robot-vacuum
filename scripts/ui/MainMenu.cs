using Godot;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.UI;

public partial class MainMenu : CanvasLayer
{
    [Export(PropertyHint.File)]
    private string _startScene = default;

    private Button _play = null;
    private Button _exit = null;

    public override void _Ready()
    {
        _play = GetNode<Button>("%Play");
        _exit = GetNode<Button>("%Exit");

        _play.Pressed += OnPlayPressed;
        _exit.Pressed += () => GetTree().Quit();
    }

    private void OnPlayPressed()
    {
        // remove all ui because transition layer does not cover it
        GetNode("Control").QueueFree();
        TransitionLayer.Instance.ChangeSceneTo(_startScene);
    }
}
