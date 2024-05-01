extends CanvasLayer

@onready var _animation_player = $"AnimationPlayer"

func _ready() -> void:
	$"ColorRect".modulate = Color(1, 1, 1, 0)

func change_scene_to_file(file: String) -> void:
	_animation_player.play("fade_to_black")
	await _animation_player.animation_finished
	get_tree().change_scene_to_file(file)
	_animation_player.play_backwards("fade_to_black")
