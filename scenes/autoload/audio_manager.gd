extends Node

# SFX
@onready var _push_garbage_player: AudioStreamPlayer = %PushGarbagePlayerSFX
@onready var _capture_garbage_player: AudioStreamPlayer = %CaptureGarbagePlayerSFX
@onready var _cat_capture_player: AudioStreamPlayer = %CatCapturePlayerSFX
@onready var _car_capture_player: AudioStreamPlayer = %CarCapturePlayerSFX
@onready var _activate_button_player: AudioStreamPlayer = %ActivateButtonPlayerSFX
@onready var _get_electricity_player: AudioStreamPlayer = %GetElectricityPlayerSFX
@onready var _wet_spot_player: AudioStreamPlayer = %WetSpotPlayerSFX
@onready var _bin_capture_player: AudioStreamPlayer = %BinCapturePlayerSFX
@onready var _next_level_ok_player: AudioStreamPlayer = %NextLevelOkPlayerSFX
@onready var _next_level_err_player: AudioStreamPlayer = %NextLevelErrorPlayerSFX
@onready var _button_click_player: AudioStreamPlayer = %ButtonClickPlayerSFX

# Music
@onready var _intro_theme_player: AudioStreamPlayer = %IntroThemePlayerMusic
@onready var _main_theme_alone_player: AudioStreamPlayer = %MainThemeAlonePlayerMusic
@onready var _main_theme_couple_player: AudioStreamPlayer = %MainThemeCouplePlayerMusic
@onready var _main_theme_family_player: AudioStreamPlayer = %MainThemeFamilyPlayerMusic
@onready var _main_theme_divorce_player: AudioStreamPlayer = %MainThemeDivorcePlayerMusic

var _current_music_type: XTypes.MusicTheme = XTypes.MusicTheme.UNKNOWN


func play_sound_robot_capture_garbage() -> void:
	_try_play(_capture_garbage_player, .2)


func play_sound_robot_push_garbage() -> void:
	_try_play(_push_garbage_player)


func play_sound_bin_capture_garbage() -> void:
	_try_play(_bin_capture_player)


func play_sound_cat_capture_robot() -> void:
	_try_play(_cat_capture_player)


func play_sound_car_capture_robot() -> void:
	_try_play(_car_capture_player)


func play_sound_button_activate() -> void:
	_try_play(_activate_button_player)


func play_sound_get_electricity() -> void:
	_try_play(_get_electricity_player)


func play_sound_wet_spot_affection() -> void:
	_try_play(_wet_spot_player)


func play_sound_level_ok() -> void:
	_try_play(_next_level_ok_player)


func play_sound_level_error() -> void:
	_try_play(_next_level_err_player)


func play_intro(do_after: Callable = func() -> void: pass ) -> void:
	_intro_theme_player.play()
	if do_after != null:
		_intro_theme_player.finished.connect(do_after)


func play_music(music_type: XTypes.MusicTheme) -> void:
	if _current_music_type == music_type:
		return

	stop_music()
	_current_music_type = music_type
	match music_type:
		XTypes.MusicTheme.ALONE:
			_main_theme_alone_player.play()
		XTypes.MusicTheme.COUPLE:
			_main_theme_couple_player.play()
		XTypes.MusicTheme.FAMILY:
			_main_theme_family_player.play()
		XTypes.MusicTheme.DIVORCE:
			_main_theme_divorce_player.play()
		_:
			print_debug("Unknown music type: %s" % music_type)


func stop_music() -> void:
	_current_music_type = XTypes.MusicTheme.UNKNOWN
	_main_theme_alone_player.stop()
	_main_theme_couple_player.stop()
	_main_theme_family_player.stop()
	_main_theme_divorce_player.stop()
	_intro_theme_player.stop()


func register_buttons(buttons: Array[Button]) -> void:
	for button: Button in buttons:
		button.pressed.connect(_on_button_pressed)


func _try_play(player: AudioStreamPlayer, start_from: float = 0) -> void:
	if player.playing:
		return
	player.play(start_from)


func _on_button_pressed() -> void:
	_try_play(_button_click_player)
