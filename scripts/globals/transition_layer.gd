extends CanvasLayer

const _anim_name := "black_screen"

@onready var _anim_player: AnimationPlayer = $AnimationPlayer
@onready var _black_rect: ColorRect = $ColorRect


func _ready() -> void:
	_black_rect.self_modulate = Color(1, 1, 1, 0)

func reload_current_scene() -> int:
	_anim_player.play(_anim_name)
	await _anim_player.animation_finished
	_anim_player.call_deferred("play_backwards", _anim_name)

	return get_tree().reload_current_scene()

func load_scene(scene: PackedScene) -> int:
	_anim_player.play(_anim_name)
	await _anim_player.animation_finished
	_anim_player.call_deferred("play_backwards", _anim_name)

	return get_tree().change_scene_to_packed(scene)
