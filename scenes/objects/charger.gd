class_name Charger

extends Area2D

# initially robot on the charger area in each level
# do not emit the signal in this case
var _can_enter: bool = false


func _ready() -> void:
	body_entered.connect(_on_body_entered)
	body_exited.connect(_on_body_exited)


func _on_body_entered(body: Node) -> void:
	if body is not Robot:
		return
	if not _can_enter:
		return
	GameEvents.robot_entered_charger_area.emit()


func _on_body_exited(body: Node) -> void:
	if body is not Robot:
		return
	_can_enter = true
