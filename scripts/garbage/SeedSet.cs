using Godot;

namespace RobotVacuum.Scripts.Garbage;

public partial class SeedSet : Node2D
{
    public override void _Process(double delta)
    {
        if (GetChildCount() == 0)
        {
            CallDeferred(Node2D.MethodName.QueueFree);
        }
    }
}
