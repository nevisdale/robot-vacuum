class_name Table

extends StaticBody2D

@onready var _sprite: Sprite2D = $Sprite2D
@onready var _hide_robot_area: Area2D = $HideRobotArea
@onready var _color_rect: ColorRect = $ColorRect


func _ready() -> void:
	# visible my be disabled in editor,
	# because it prevents editor choose other nodes
	_color_rect.visible = true

	_hide_robot_area.body_entered.connect(_hide_robot_area_on_body_entered)
	_hide_robot_area.body_exited.connect(_hide_robot_area_on_body_exited)


func _hide_robot_area_on_body_entered(body: Node) -> void:
	var robot := body as Robot
	if robot != null:
		robot.set_can_be_captured_by_enemy(false)
		_set_table_visibility(false)


func _hide_robot_area_on_body_exited(body: Node) -> void:
	var robot := body as Robot
	if robot != null:
		robot.set_can_be_captured_by_enemy(true)
		_set_table_visibility(true)


func _set_table_visibility(visibility: bool) -> void:
	var color_rect_alpha: float = 0
	var sprite_alpha: float = 1

	if not visibility:
		color_rect_alpha = .2
		sprite_alpha = .5

	const TWEEN_DURATION = .5

	var tween := create_tween()
	tween.set_parallel(true)
	tween.tween_property(_sprite, "modulate:a", sprite_alpha, TWEEN_DURATION)
	tween.tween_property(_color_rect, "color:a", color_rect_alpha, TWEEN_DURATION)
