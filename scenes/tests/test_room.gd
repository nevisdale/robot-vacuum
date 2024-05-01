extends Node2D

@onready var _garbage_container: Node2D = $"GarbageContainer"
@onready var _garbage_markers: Node2D = $"GarbageMarkers"
@onready var _camera: Camera2D = $"Robot/Camera2D"
# @onready var _robot: Robot = $"Robot"
@onready var _charge_station: Area2D = $"ChargeStation"
# @onready var _ui: UI = $"UI"

var _garbage_scenes: Array[PackedScene] = [
	preload ("res://scenes/garbage/seed_1.tscn"),
	preload ("res://scenes/garbage/seed_2.tscn"),
	preload ("res://scenes/garbage/seed_3.tscn"),
	preload ("res://scenes/garbage/cola.tscn"),
]

func _ready() -> void:
	_charge_station.body_entered.connect(_change_station_on_body_entrered)
	ready.connect(_after_ready)

func _after_ready() -> void:
	_camera.limit_smoothed = true
	_camera.position_smoothing_enabled = true

func _process(_delta: float) -> void:
	Globals.garbage_count = _garbage_container.get_child_count(false)
	
	if Input.is_key_pressed(KEY_R):
		TransitionLayer.change_scene_to_file("res://scenes/tests/test_room.tscn")

	# print(current_scene)

func _init_garbage() -> void:
	for marker in _garbage_markers.get_children(false):
		marker = marker as Marker2D
		for i in range(3):
			_create_seed(marker.position + Vector2.LEFT.rotated(randf_range(0, 2 * PI)) * randf() * 100)
		
func _create_seed(pos: Vector2) -> void:
	var garbage_instance = _garbage_scenes.pick_random().instantiate()
	garbage_instance.position = pos
	garbage_instance.rotation = randf_range(0, 2 * PI)
	_garbage_container.call_deferred("add_child", garbage_instance)

func _change_station_on_body_entrered(_body: Node2D) -> void:
	if Globals.garbage_count == 0:
		_init_garbage()
