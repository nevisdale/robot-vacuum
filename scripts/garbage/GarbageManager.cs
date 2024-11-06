using System.Diagnostics;
using Godot;

namespace RobotVacuum.Scripts.Garbage;

public partial class GarbageManager : Node
{
	private const string GROUP_IS_GARBAGE = "is_garbage";

	public static bool IsGarbage(Node node)
	{
		return node.IsInGroup(GROUP_IS_GARBAGE);
	}

	public static IGarbage GetGarbageOrNull(Node node)
	{
		if (IsGarbage(node))
		{
			IGarbage garbage = node as IGarbage;
			Debug.Assert(garbage != null, $"{node.Name} in group {GROUP_IS_GARBAGE} must implement IGarbage");
			return garbage;
		}
		return null;
	}
}
