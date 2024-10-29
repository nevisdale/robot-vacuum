extends StaticBody2D

class_name Bin

@onready var _capture_are: Area2D = $GarbageCaptureArea

func _ready() -> void:
	_capture_are.body_entered.connect(_try_capture_garbage)

func _try_capture_garbage(body: Node2D) -> void:
	if not Garbage.is_garbage(body):
		return

	var garbage := body
	var capturer := self
	if Garbage.can_be_captured_by_bin(garbage):
		Garbage.capture(garbage, capturer)
