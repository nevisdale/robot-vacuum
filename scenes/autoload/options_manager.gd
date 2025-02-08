extends Node

var _bus_name_music: String = "Music"
var _bus_name_sfx: String = "SFX"


func _ready() -> void:
	_set_bus_volume_percent(_bus_name_music, SaveManager.get_music_volume())
	_set_bus_volume_percent(_bus_name_sfx, SaveManager.get_sfx_volume())


func is_fullscreen() -> bool:
	return (
		DisplayServer.window_get_mode() == DisplayServer.WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN
	)


func toggle_window_mode() -> void:
	print_debug("Toggle window mode")
	if is_fullscreen():
		DisplayServer.window_set_mode(DisplayServer.WindowMode.WINDOW_MODE_WINDOWED)
	else:
		DisplayServer.window_set_mode(DisplayServer.WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN)
	GameEvents.options_window_mode_changed.emit()


func enable_mouse() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)


func disable_mouse() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_HIDDEN)


func get_music_volume_percent() -> float:
	return _get_bus_volume_percent(_bus_name_music)


func add_music_volume_percent(percent: float) -> void:
	_set_bus_volume_percent(_bus_name_music, get_music_volume_percent() + percent)
	SaveManager.set_music_volume(get_music_volume_percent())


func get_sfx_volume_percent() -> float:
	return _get_bus_volume_percent(_bus_name_sfx)


func add_sfx_volume_percent(percent: float) -> void:
	_set_bus_volume_percent(_bus_name_sfx, get_sfx_volume_percent() + percent)
	SaveManager.set_sfx_volume(get_sfx_volume_percent())


func _get_bus_volume_percent(bus_name: String) -> float:
	var bus_index := AudioServer.get_bus_index(bus_name)
	return db_to_linear(AudioServer.get_bus_volume_db(bus_index))


func _set_bus_volume_percent(bus_name: String, percent: float) -> void:
	percent = clamp(percent, 0.0, 1.0)
	var bus_index := AudioServer.get_bus_index(bus_name)
	AudioServer.set_bus_volume_db(bus_index, linear_to_db(percent))
