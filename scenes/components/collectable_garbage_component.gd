class_name CollectableGarbageComponent

extends Node2D

var _capturer: Node2D


func _process(delta: float) -> void:
	if _capturer != null:
		var owner2d := owner as Node2D
		assert(owner2d != null)
		owner2d.global_position = owner2d.global_position.lerp(_capturer.global_position, delta * 5)


func can_be_captured_by_bin() -> bool:
	return false


func can_be_captured_by_robot() -> bool:
	return true


func capture(capturer: Node2D) -> void:
	_capturer = capturer

	const SCALE_FACTOR = .2
	const TWEEN_DURATION = .3

	var tween := create_tween()
	tween.set_parallel(true)
	tween.tween_property(owner, "scale", scale * SCALE_FACTOR, TWEEN_DURATION)

	tween.finished.connect(owner.queue_free)
