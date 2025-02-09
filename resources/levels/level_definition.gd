class_name LevelDefinition

extends Resource

const AudioManager := preload("res://scenes/autoload/audio_manager.gd")

@export var level_id: String
@export_file("*.tscn") var level_scene_file_path: String
@export var music_theme: AudioManager.MusicTheme
@export var is_intro: bool
