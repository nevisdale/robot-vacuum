extends StaticBody2D

class_name Table

@onready var _sprite: Sprite2D = $Sprite2D
@onready var _hide_robot_area: Area2D = $HideRobotArea

func _ready() -> void:
	_hide_robot_area.body_entered.connect(_try_hide_robot)
	_hide_robot_area.body_exited.connect(_try_show_robot)

func _try_hide_robot(body: Node2D) -> void:
	var robot: Robot = body as Robot
	assert(robot != null, "Table can only hide Robot")

	_make_robot_visible(robot, false)


func _try_show_robot(body: Node2D) -> void:
	var robot: Robot = body as Robot
	assert(robot != null, "Table can only hide Robot")

	_make_robot_visible(robot, true)


func _make_robot_visible(robot: Robot, value: bool) -> void:
	robot.make_visible(value)

	const HIDE_ALPHA: float = 0.5
	const SHOW_ALPHA: float = 1.0
	const DURATION: float = 0.5

	var alpha := SHOW_ALPHA
	if not value:
		alpha = HIDE_ALPHA
	var tween := create_tween()
	tween.tween_property(_sprite, "self_modulate:a", alpha, DURATION)
