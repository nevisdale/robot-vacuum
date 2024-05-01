extends Area2D

func _ready() -> void:
	var tween := create_tween()
	tween.tween_property(self, "scale", scale, 0.1).from(Vector2.ZERO)
