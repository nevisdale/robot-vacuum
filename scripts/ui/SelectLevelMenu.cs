using Godot;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.UI;

public partial class SelectLevelMenu : CanvasLayer
{
    [Export(PropertyHint.File)]
    private string _mainMenuScene = default;

    private Button _back = null;

    public override void _Ready()
    {
        _back = GetNode<Button>("%Back");
        _back.Pressed += () => { GoToMainMenu(); };
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            GoToMainMenu();
        }
    }

    public void GoToMainMenu()
    {
        TransitionLayer.Instance.ChangeSceneToFast(_mainMenuScene);
    }
}
