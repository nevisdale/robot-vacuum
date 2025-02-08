class_name IntroLevel

extends Node2D

@export var _level_definition: LevelDefinition

@onready var robot: Robot = $Robot
@onready var _camera: GameCamera = $Robot/GameCamera
@onready var _ear_icon: TextureRect = %EarIconTextureRect
@onready var _start_audio_timer: Timer = %StartAudioTimer
@onready var _black_rect: ColorRect = $BlackColorRect
@onready var _pause_menu: PauseMenu = $PauseMenu


func _ready() -> void:
	SaveManager.set_level_to_continue(_level_definition.level_id)
	AudioManager.stop_music()
	OptionsManager.disable_mouse()
	_pause_menu.set_level_title("Intro")

	robot.make_not_moveable()
	_camera.reset_smoothing()

	_ear_icon.modulate.a = .2
	var tween := create_tween()
	tween.set_loops(3)
	tween.tween_property(_ear_icon, "modulate:a", .8, .3)
	tween.tween_property(_ear_icon, "modulate:a", .2, .3)
	tween.tween_property(_ear_icon, "modulate:a", .8, .3)

	_start_audio_timer.start()
	await _start_audio_timer.timeout

	AudioManager.play_intro(_do_after_intro_music_theme)


func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("next_level"):
		LevelManager.go_to_next_level()
		get_viewport().set_input_as_handled()
	if event.is_action_pressed("fullscreen"):
		OptionsManager.toggle_window_mode()
		get_viewport().set_input_as_handled()
	if event.is_action_pressed("restart"):
		LevelManager.reload_level()
		get_viewport().set_input_as_handled()


func _do_after_intro_music_theme() -> void:
	var tween := create_tween()
	tween.tween_property(_black_rect, "modulate:a", 0, 1)
	tween.tween_property(_camera, "zoom", Vector2(1, 1), 2)
	tween.tween_property(_camera, "zoom", Vector2(.55, .55), 2)
	await tween.finished

	LevelManager.go_to_next_level()
