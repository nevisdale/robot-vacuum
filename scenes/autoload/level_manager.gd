extends Node

@export var _level_definitions: Array[LevelDefinition]
@export_file("*.tscn") var _main_menu_scene_path: String
@export_file("*.tscn") var _select_level_scene_path: String

var _current_level_index: int = 0


func go_to_next_level() -> void:
	_current_level_index += 1
	if _current_level_index >= len(_level_definitions):
		return
	var definition := _level_definitions[_current_level_index]
	LevelTransitionManager.change_scene_to_file(definition.level_scene_file_path)


func update_current_level_index(level_id: String) -> void:
	var index := _get_level_definition_index_by_id(level_id)
	if index == -1:
		push_error("Level definition not found: id: %s" % level_id)
		return
	_current_level_index = index


func load_level(level_id: String) -> void:
	var index := _get_level_definition_index_by_id(level_id)
	if index == -1:
		return
	var level_scene := _level_definitions[index].level_scene_file_path
	_current_level_index = index
	LevelTransitionManager.change_scene_to_file(level_scene)


func get_level_index() -> int:
	return _current_level_index


func reload_level() -> void:
	LevelTransitionManager.reload_current_scene()


func get_all_level_definitions() -> Array[LevelDefinition]:
	return _level_definitions


func go_to_main_menu(animate: bool = true) -> void:
	LevelTransitionManager.change_scene_to_file(_main_menu_scene_path, animate)


func go_to_select_level_menu(animate: bool = true) -> void:
	LevelTransitionManager.change_scene_to_file(_select_level_scene_path, animate)


func reset_and_start() -> void:
	_current_level_index = 0
	var level_scene := _level_definitions[_current_level_index].level_scene_file_path
	LevelTransitionManager.change_scene_to_file(level_scene)


func _get_level_definition_index_by_id(level_id: String) -> int:
	for i in range(len(_level_definitions)):
		if _level_definitions[i].level_id == level_id:
			return i
	return -1
