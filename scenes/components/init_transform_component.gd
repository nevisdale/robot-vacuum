class_name InitTransformComponent

extends Node2D

@export var random_rotation: bool = true


func _ready() -> void:
	var owner2d := owner as Node2D
	if owner2d == null:
		return

	if random_rotation:
		owner2d.rotate(randf() * PI * 2)
