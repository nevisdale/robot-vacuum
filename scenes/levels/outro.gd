class_name OutroLevel

extends Node2D

@export var _level_definition: LevelDefinition

@onready var _robot: Robot = $Robot
@onready var _camera: GameCamera = $Robot/GameCamera
@onready var _ear_icon: TextureRect = %EarIconTextureRect
@onready var _directioinal_light: DirectionalLight2D = $DirectionalLight2D
@onready var _thank_you_label: Label = %ThankYouForPlayingLabel
@onready var _pause_menu: PauseMenu = $PauseMenu


func _ready() -> void:
	OptionsManager.disable_mouse()
	AudioManager.stop_music()
	SaveManager.set_can_choose_level(_level_definition.level_id)
	SaveManager.set_level_to_continue(_level_definition.level_id)
	_pause_menu.set_level_title("Outro")

	_thank_you_label.visible = false
	_ear_icon.visible = false
	_camera.reset_smoothing()

	var tween := create_tween()
	tween.tween_property(_directioinal_light, "color:a", 1, 15)
	await tween.finished

	# this box can overlap robot lights.
	# red light must be visible in this scene
	get_node("Env/BoxAngleOpen").queue_free()

	tween = create_tween()
	tween.tween_property(_camera, "zoom", Vector2(1, 1), 5)
	await tween.finished

	_ear_icon.visible = true
	_ear_icon.modulate.a = .2
	tween = create_tween()
	tween.set_loops(3)
	tween.tween_property(_ear_icon, "modulate:a", .8, .3)
	tween.tween_property(_ear_icon, "modulate:a", .2, .3)
	tween.tween_property(_ear_icon, "modulate:a", .8, .3)

	AudioManager.play_intro(_do_after_intro)

	await get_tree().create_timer(9).timeout
	_robot.in_danger_area()

	await get_tree().create_timer(4).timeout
	_robot.make_not_moveable()
	_robot.disable_light()


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


func _do_after_intro() -> void:
	var tween := create_tween()
	tween.tween_property(_ear_icon, "modulate:a", 0, 1)
	await tween.finished

	_thank_you_label.visible = true
	_thank_you_label.modulate.a = 0

	tween = create_tween()
	tween.tween_property(_thank_you_label, "modulate:a", 0.8, 2)
	tween.tween_interval(2)
	tween.tween_property(_thank_you_label, "modulate:a", 0, 2)
	await tween.finished

	LevelManager.go_to_main_menu()
