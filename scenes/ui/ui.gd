extends CanvasLayer

class_name UI

@onready var _label: Label = $"MarginContainer/TaskContainer/Label"

func _ready() -> void:
	Globals.garbage_count_changed.connect(_on_garbage_count_changed)

func _on_garbage_count_changed(value) -> void:
	if value == 0:
		_label.text = "Go to the charge station"
	else:
		_label.text = "Clean the room: (" + str(value) + ")"
	