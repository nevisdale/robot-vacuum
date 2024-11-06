using Godot;

namespace RobotVacuum.Scripts.Garbage;

public interface IGarbage
{
	bool CanBeCapturedByRobot();
	bool CanBeCapturedByBin();
	void Capture(Node2D capturer);
}
