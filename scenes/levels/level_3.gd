class_name Level3
extends LevelBase

func _ready() -> void:
	super._ready()
	currenct_level_path = "res://scenes/levels/level_3.tscn"
	next_level_path = "res://scenes/levels/level_4.tscn"

	for node in garbage_container.get_children(false):
		node.rotation = randf_range(0, 2 * PI)

func _process(delta: float) -> void:
	super._process(delta)
