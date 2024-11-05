using Godot;

namespace RobotVacuum.Scripts.Globals;

public partial class State : Node
{
    public static State Instance { get; private set; }

    public int GarbageLeft { get; set; } = 0;

    public override void _Ready() => Instance = this;
}
