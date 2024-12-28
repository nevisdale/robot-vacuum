using Godot;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.UI;

public partial class LevelButton : Button
{
    [Export]
    private string _title = "";
    [Export(PropertyHint.File)]
    private string _scenePath = "";

    public override void _Ready()
    {
        Text = _title;
        Disabled = MustBeDisabled();
        Pressed += OnPressed;
    }

    public override void _Process(double delta)
    {
        Disabled = MustBeDisabled();
    }

    private void OnPressed()
    {
        TransitionLayer.Instance.ChangeSceneTo(_scenePath);
    }

    private bool MustBeDisabled()
    {
        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        return !gameState.AvailableLevelScenes.Contains(_scenePath);
    }
}
