class_name MainMenu

extends CanvasLayer

@onready var _play_button: Button = %PlayButton
@onready var _continue_button: Button = %ContinueButton
@onready var _select_level_button: Button = %SelectLevelButton
@onready var _window_mode_button: Button = %WindowModeButton
@onready var _exit_button: Button = %ExitButton


func _ready() -> void:
	OptionsManager.enable_mouse()
	_update_visual()

	AudioManager.register_buttons(
		[_play_button, _continue_button, _select_level_button, _window_mode_button, _exit_button]
	)

	_play_button.pressed.connect(_on_play_button_pressed)
	_continue_button.pressed.connect(_on_continue_button_pressed)
	_select_level_button.pressed.connect(_on_select_level_button_pressed)
	_window_mode_button.pressed.connect(_on_window_mode_button_pressed)
	_exit_button.pressed.connect(_on_exit_button_pressed)

	GameEvents.options_window_mode_changed.connect(_update_visual)


func _unhandled_input(_event: InputEvent) -> void:
	if Input.is_action_just_pressed("fullscreen"):
		OptionsManager.toggle_window_mode()
		set_process_input(true)


func _on_play_button_pressed() -> void:
	LevelManager.reset_and_start()


func _on_continue_button_pressed() -> void:
	var continue_level := SaveManager.get_level_to_continue()
	if continue_level != "":
		LevelManager.load_level(continue_level)


func _on_select_level_button_pressed() -> void:
	LevelManager.go_to_select_level_menu(false)


func _on_window_mode_button_pressed() -> void:
	OptionsManager.toggle_window_mode()
	_update_visual()
	_window_mode_button.focus_mode = Control.FOCUS_NONE


func _on_exit_button_pressed() -> void:
	get_tree().quit()


func _update_visual() -> void:
	var level_to_continue := SaveManager.get_level_to_continue()
	_continue_button.disabled = level_to_continue == ""

	var window_mode_text := "Fullscreen"
	if OptionsManager.is_fullscreen():
		window_mode_text = "Windowed"
	_window_mode_button.text = window_mode_text
