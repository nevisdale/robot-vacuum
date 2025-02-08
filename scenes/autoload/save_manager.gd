extends Node

const FILE_PATH = "user://data.json"

var _save_data: SaveData


func get_level_to_continue() -> String:
	return _save_data.current_level


func set_level_to_continue(level_id: String) -> void:
	_save_data.current_level = level_id
	_save()


func get_can_choose_level(level_id: String) -> bool:
	return _save_data.can_choose_levels.has(level_id)


func set_can_choose_level(level_id: String) -> void:
	_save_data.can_choose_levels[level_id] = true
	_save()


func get_music_volume() -> float:
	return _save_data.music_volume


func set_music_volume(volume: float) -> void:
	_save_data.music_volume = volume
	_save()


func get_sfx_volume() -> float:
	return _save_data.sfx_volume


func set_sfx_volume(volume: float) -> void:
	_save_data.sfx_volume = volume
	_save()


func _ready() -> void:
	_save_data = SaveData.new()
	_save_data.can_choose_levels = {}

	var ok := _load()
	if not ok:
		# create default save data
		_save_data.music_volume = 0.5
		_save_data.sfx_volume = 0.5
		_save()


func _load() -> bool:
	var file := FileAccess.open(FILE_PATH, FileAccess.READ)
	if file == null:
		push_warning("Could not open file for reading: %s" % FILE_PATH)
		return false
	var data_raw: Variant = JSON.parse_string(file.get_as_text())
	var parsed := SaveData.from_data(_save_data, data_raw)
	return parsed


func _save() -> void:
	var json_data := JSON.stringify(_save_data.as_dict())
	var file := FileAccess.open(FILE_PATH, FileAccess.WRITE)
	if file == null:
		push_error("Could not open file for writing: %s" % FILE_PATH)
		return
	file.store_string(json_data)
	file.flush()
