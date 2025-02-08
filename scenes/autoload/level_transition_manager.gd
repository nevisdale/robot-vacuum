extends CanvasLayer

const ANIM_NAME = "black_screen"

@onready var _animation_player: AnimationPlayer = $AnimationPlayer
@onready var _black_rect: ColorRect = $ColorRect


func _ready() -> void:
	_black_rect.self_modulate = Color(1, 1, 1, 0)
	hide()


func is_active() -> bool:
	return visible == true


func reload_current_scene(animate: bool = true) -> void:
	var fn := get_tree().reload_current_scene
	_animate_and_do(fn, animate)


func change_scene_to_file(scene_path: String, animate: bool = true) -> void:
	var fn := func() -> void: get_tree().change_scene_to_file(scene_path)
	_animate_and_do(fn, animate)


func change_scene_to_packed(packed_scene: PackedScene, animate: bool = true) -> void:
	var fn := func() -> void: get_tree().change_scene_to_packed(packed_scene)
	_animate_and_do(fn, animate)


func _animate_and_do(fn: Callable, animate: bool) -> void:
	# show and hide because the animation doesn't work if the node is not visible
	# this layer is grather than the other layers, so it will be on top of everything
	if animate:
		show()

		_animation_player.play(ANIM_NAME)
		await _animation_player.animation_finished

		var back_anim_and_hide := func() -> void:
			_animation_player.play_backwards(ANIM_NAME)
			await _animation_player.animation_finished
			hide()
		back_anim_and_hide.call_deferred()

	fn.call()
