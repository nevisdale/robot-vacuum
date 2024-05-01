extends CharacterBody2D

enum State {
	ATTACK,
	IDLE
}

@onready var _follow_2d: PathFollow2D = get_parent()
@onready var _robot_capture_area: Area2D = $"Area2D"

var state: State = State.IDLE

func _ready() -> void:
	_robot_capture_area.body_entered.connect(_on_body_entered)

func _physics_process(delta: float) -> void:
	if state == State.IDLE:
		_follow_2d.progress_ratio += 0.1 * delta

func _on_body_entered(body: Node2D) -> void:
	if body.invisible == true:
		return
		
	state = State.ATTACK

	var tween := create_tween()
	tween.tween_property(self, "global_position", body.global_position, 0.3)
	await tween.finished
	get_tree().reload_current_scene()