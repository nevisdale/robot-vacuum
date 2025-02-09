class_name FloorButtonComponent

extends Node2D

enum Direction { LEFT, RIGHT }

@export var _button_area: Area2D
@export var _direction: Direction = Direction.LEFT

# several obejcts can press the button at the same time
# so we need to keep track of how many objects are pressing the button
# to be sure that the button is pressed and released correctly
var _pressed_count: int = 0


func _ready() -> void:
	assert(_button_area != null, "FloorButtonComponent: _button_area is not set")

	_button_area.body_entered.connect(_on_body_entered)
	_button_area.body_exited.connect(_on_body_exited)


func _on_body_entered(_body: Node) -> void:
	_pressed_count += 1
	if _pressed_count == 1:
		_button_just_pressed()


func _on_body_exited(_body: Node) -> void:
	_pressed_count -= 1


func _button_just_pressed() -> void:
	AudioManager.play_sound_button_activate()
	match _direction:
		Direction.LEFT:
			GameEvents.floor_button_left_just_pressed.emit()
		Direction.RIGHT:
			GameEvents.floor_button_right_just_pressed.emit()
