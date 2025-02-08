class_name LevelButton

extends Button

signal level_button_pressed


func _ready() -> void:
	pressed.connect(_on_pressed)


func set_title(title: String) -> void:
	self.text = title


func _on_pressed() -> void:
	level_button_pressed.emit()
