extends Area2D

class_name Seed

func _ready() -> void:
	rotate(randf_range(-PI, PI))

# see Garbage.is_garbage for more details
func is_garbage() -> void:
	pass

func can_be_captured_by_robot() -> bool:
	return true

func can_be_captured_by_bin() -> bool:
	return false

func capture(capturer: Node2D) -> void:
	const tween_duration := 0.3
	const scale_factor := 0.2

	var tween := create_tween()
	tween.set_parallel(true)
	tween.finished.connect(queue_free)
	tween.tween_property(self, "global_position", capturer.global_position, tween_duration)
	tween.tween_property(self, "scale", scale * scale_factor, tween_duration)
