class_name LevelTemplate

extends Node

@export var _level_definition: LevelDefinition

@onready var robot: Robot = $Robot
@onready var _garbage_container: Node2D = $Garbage
@onready var _camera: GameCamera = %GameCamera
@onready var _pause_menu: PauseMenu = $PauseMenu

var _room_is_cleaned: bool = false


func _ready() -> void:
	OptionsManager.disable_mouse()

	if _level_definition != null:
		SaveManager.set_level_to_continue(_level_definition.level_id)
		SaveManager.set_can_choose_level(_level_definition.level_id)
		LevelManager.update_current_level_index(_level_definition.level_id)
		AudioManager.play_music(_level_definition.music_theme)
		var level_index := LevelManager.get_level_index()
		_pause_menu.set_level_title("Level %s" % str(level_index))

	_garbage_container.child_exiting_tree.connect(_on_garbage_container_child_exiting_tree)
	GameEvents.robot_entered_charger_area.connect(_on_robot_entered_charger_area)
	GameEvents.robot_captured_by_enemy.connect(_on_robot_captured_by_enemy)
	GameEvents.robot_received_electricity.connect(_on_robot_received_electricity)

	_camera.reset_smoothing()


func _unhandled_input(event: InputEvent) -> void:
	if OS.is_debug_build() and event.is_action_pressed("next_level"):
		LevelManager.go_to_next_level()
		get_viewport().set_input_as_handled()


func _on_garbage_container_child_exiting_tree(_node: Node) -> void:
	_check_room_is_cleaned.call_deferred()


func _check_room_is_cleaned() -> void:
	if _garbage_container.get_children().size() == 0:
		_room_is_cleaned = true


func _on_robot_captured_by_enemy(enemy: Node) -> void:
	_camera.shake()
	LevelTransitionManager.reload_current_scene()
	print_debug("Robot captured by enemy(%s)" % enemy.name)


func _on_robot_entered_charger_area() -> void:
	if _room_is_cleaned:
		print_debug("level is completed")
		AudioManager.play_sound_level_ok()
		LevelManager.go_to_next_level()
	else:
		_camera.shake()
		AudioManager.play_sound_level_error()
		print_debug("level is not completed yet")


func _on_robot_received_electricity() -> void:
	_camera.shake()
