extends StaticBody2D

@onready var _garbage_area: Area2D = $"GarbageArea"

func _ready() -> void:
	_garbage_area.body_entered.connect(_on_body_entered)

func _on_body_entered(body: Node2D) -> void:
	var tween := create_tween().set_parallel(true)
	tween.tween_property(body, "scale", Vector2.ZERO, 0.2)
	tween.tween_property(body, "position", position, 0.2)
	tween.finished.connect(body.queue_free)
	Globals.garbage_count -= 1
