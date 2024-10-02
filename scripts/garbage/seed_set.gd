extends Node2D

class_name SeedSet


func _process(_delta: float) -> void:
	if get_child_count() == 0:
		call_deferred("queue_free")
