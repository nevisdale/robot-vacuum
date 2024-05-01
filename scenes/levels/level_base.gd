class_name LevelBase

extends Node2D

@onready var garbage_container: Node2D = $"GarbageContainer"
@onready var camera: Camera2D = $"Robot/Camera2D"
@onready var charge_station: Area2D = $"ChargeStation"

var currenct_level_path: String = "res://scenes/levels/level_base.tscn"
var next_level_path: String = ""

func _ready() -> void:
	ready.connect(_after_ready)
	charge_station.body_entered.connect(_on_body_entered)
	Globals.garbage_count = garbage_container.get_child_count(false)

func _after_ready() -> void:
	camera.limit_smoothed = true
	camera.position_smoothing_enabled = true

func _process(_delta: float) -> void:
	Globals.garbage_count = garbage_container.get_child_count(false)
	
	if Input.is_key_pressed(KEY_R):
		TransitionLayer.change_scene_to_file(currenct_level_path)

func _on_body_entered(_body: Node2D) -> void:
	if Globals.garbage_count == 0 and next_level_path.length() > 0:
		TransitionLayer.change_scene_to_file(next_level_path)