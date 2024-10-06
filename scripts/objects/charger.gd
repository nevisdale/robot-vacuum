extends Area2D

class_name Charger

signal level_complete

func _ready() -> void:
	self.body_entered.connect(_robot_entered)


func _robot_entered(_robot: Node2D) -> void:
	assert(_robot is Robot, "charger area can only be entered by a robot")

	if GlobalState.garbage_left == 0:
		level_complete.emit()
