using Godot;

namespace Garbage;

public interface IGarbage {
	public bool CanBeCapturedByRobot();
	public bool CanBeCapturedByBin();
	public void CaptureAndDestroy(Vector2 capturerGlobalPosition);
}
