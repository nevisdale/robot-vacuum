class_name RobotDangerAreaComponent

extends Area2D


func _ready() -> void:
	body_entered.connect(_on_body_entered)
	body_exited.connect(_on_body_exited)


func _on_body_entered(body: Node2D) -> void:
	var robot := body as Robot
	if robot != null:
		robot.in_danger_area()


func _on_body_exited(body: Node2D) -> void:
	var robot := body as Robot
	if robot != null:
		robot.out_danger_area()
