class_name SaveData

const CAN_CHOOSE_LEVELS := "can_choose_levels"
const CURRENT_LEVEL_KEY := "current_level"
const MUSIC_VOLUME_KEY := "music_volume"
const SFX_VOLUME_KEY := "sfx_volume"

var can_choose_levels: Dictionary = {}
var current_level: String
var music_volume: float
var sfx_volume: float


func as_dict() -> Dictionary:
	return {
		CAN_CHOOSE_LEVELS: can_choose_levels.keys(),
		CURRENT_LEVEL_KEY: current_level,
		MUSIC_VOLUME_KEY: music_volume,
		SFX_VOLUME_KEY: sfx_volume,
	}


static func from_data(src: SaveData, data: Variant) -> bool:
	if data is not Dictionary:
		push_error("Invalid save data")
		return false

	var dct: Dictionary = data
	for key: String in dct.keys():
		match key:
			CAN_CHOOSE_LEVELS:
				src.can_choose_levels.clear()
				var level_ids: Variant = dct.get(key, [])
				# breakpoint
				if level_ids is not Array:
					push_warning("Invalid value for completed levels in save data")
					continue
				for level_id: String in level_ids:
					src.can_choose_levels[level_id] = true

			CURRENT_LEVEL_KEY:
				src.current_level = dct[key]

			MUSIC_VOLUME_KEY:
				src.music_volume = dct[key]

			SFX_VOLUME_KEY:
				src.sfx_volume = dct[key]

			_:
				push_warning("Invalid key in save data: %s" % key)
	return true
