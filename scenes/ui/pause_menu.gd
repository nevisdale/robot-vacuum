class_name PauseMenu

extends CanvasLayer

@onready var _continue_button: Button = %ContinueButton
@onready var _restart_button: Button = %RestartButton
@onready var _sound_label: Label = %SoundLabel
@onready var _sound_less_button: Button = %SoundLessButton
@onready var _sound_more_button: Button = %SoundMoreButton
@onready var _music_label: Label = %MusicLabel
@onready var _music_less_button: Button = %MusicLessButton
@onready var _music_more_button: Button = %MusicMoreButton
@onready var _window_mode_button: Button = %WindowModeButton
@onready var _menu_button: Button = %MenuButton
@onready var _exit_button: Button = %ExitButton
@onready var _title_label: Label = %TitleLabel


func _ready() -> void:
	_update_visuals()
	AudioManager.register_buttons(
		[
			_continue_button,
			_restart_button,
			_sound_less_button,
			_sound_more_button,
			_music_less_button,
			_music_more_button,
			_window_mode_button,
			_menu_button,
			_exit_button
		]
	)

	_continue_button.pressed.connect(_on_continue_button_pressed)
	_restart_button.pressed.connect(_on_restart_button_pressed)
	_sound_less_button.pressed.connect(_on_sound_less_button_pressed)
	_sound_more_button.pressed.connect(_on_sound_more_button_pressed)
	_music_less_button.pressed.connect(_on_music_less_button_pressed)
	_music_more_button.pressed.connect(_on_music_more_button_pressed)
	_window_mode_button.pressed.connect(_on_window_mode_button_pressed)
	_menu_button.pressed.connect(_on_menu_button_pressed)
	_exit_button.pressed.connect(_on_exit_button_pressed)

	GameEvents.options_window_mode_changed.connect(_update_visuals)


func set_level_title(title: String) -> void:
	_title_label.text = title


func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("pause"):
		if visible == false:
			_show_pause_menu()
		else:
			_hide_pause_menu()
		get_viewport().set_input_as_handled()

	if event.is_action_pressed("fullscreen"):
		OptionsManager.toggle_window_mode()
		get_viewport().set_input_as_handled()

	if event.is_action_pressed("restart"):
		_restart_level()
		get_viewport().set_input_as_handled()


func _show_pause_menu() -> void:
	if LevelTransitionManager.is_active():
		return
	OptionsManager.enable_mouse()
	show()
	get_tree().paused = true


func _hide_pause_menu() -> void:
	if LevelTransitionManager.is_active():
		return
	OptionsManager.disable_mouse()
	hide()
	get_tree().paused = false


func _restart_level() -> void:
	_hide_pause_menu()
	LevelManager.reload_level()


func _on_continue_button_pressed() -> void:
	_hide_pause_menu()


func _on_restart_button_pressed() -> void:
	_restart_level()


func _on_sound_less_button_pressed() -> void:
	OptionsManager.add_sfx_volume_percent(-0.1)
	_update_visuals()


func _on_sound_more_button_pressed() -> void:
	OptionsManager.add_sfx_volume_percent(0.1)
	_update_visuals()


func _on_music_less_button_pressed() -> void:
	OptionsManager.add_music_volume_percent(-0.1)
	_update_visuals()


func _on_music_more_button_pressed() -> void:
	OptionsManager.add_music_volume_percent(0.1)
	_update_visuals()


func _on_window_mode_button_pressed() -> void:
	OptionsManager.toggle_window_mode()
	_update_visuals()
	_window_mode_button.focus_mode = Control.FOCUS_NONE


func _on_menu_button_pressed() -> void:
	_hide_pause_menu()

	OptionsManager.enable_mouse()
	AudioManager.stop_music()
	LevelManager.go_to_main_menu(false)


func _on_exit_button_pressed() -> void:
	AudioManager.stop_music()
	get_tree().quit()


func _update_visuals() -> void:
	_sound_label.text = "Sound: " + str(round(OptionsManager.get_sfx_volume_percent() * 10))
	_music_label.text = "Music: " + str(round(OptionsManager.get_music_volume_percent() * 10))

	var window_mode_text := "Windowed [F]"
	if not OptionsManager.is_fullscreen():
		window_mode_text = "Fullscreen [F]"
	_window_mode_button.text = window_mode_text
