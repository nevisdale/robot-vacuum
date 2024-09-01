using System.Diagnostics;
using System.IO;
using Godot;
using Objects;

namespace Enemies;

public partial class Cat : Node2D {
    private Sprite2D sprite;
    private Area2D captureArea;

    [Export]
    private PathFollow2D pathFollow;

    [Export]
    private float speed = 200f;


    public override void _Ready() {
        Debug.Assert(pathFollow != null, "PathFollow is not set");
        GlobalPosition = pathFollow.GlobalPosition;

        sprite = GetNode<Sprite2D>("Sprite2D");
        captureArea = GetNode<Area2D>("CaptureArea");
        captureArea.BodyEntered += CaptureArea_BodyEntered;
    }

    public override void _Process(double delta) {
        pathFollow.Progress += speed * (float)delta;
        GlobalPosition = pathFollow.GlobalPosition;
    }

    private void CaptureArea_BodyEntered(Node2D body) {
        Debug.Assert(body is Robot, "Cat can only capture robots");
        var robot = (Robot)body;

        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, Node2D.PropertyName.GlobalPosition.ToString(), robot.GlobalPosition, 0.2f);

        tween.Finished += () => {
            ProcessMode = ProcessModeEnum.Disabled;
            robot.Destroy();
        };
    }
}
