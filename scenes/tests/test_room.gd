extends Node2D

@onready var _garbage_container: Node2D = $"GarbageContainer"

var _seed_scenes: Array[PackedScene] = [
	preload ("res://scenes/garbage/seed_1.tscn"),
	preload ("res://scenes/garbage/seed_2.tscn"),
	preload ("res://scenes/garbage/seed_3.tscn"),
]

func _ready() -> void:
	var viewport_size: Vector2 = get_viewport_rect().size
	for i in range(100):
		var seed_instance = _seed_scenes.pick_random().instantiate()
		seed_instance.position = Vector2(
			randf_range(0, viewport_size.x),
			randf_range(0, viewport_size.y),
		)
		_garbage_container.add_child(seed_instance)
