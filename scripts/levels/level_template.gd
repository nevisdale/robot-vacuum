extends Node2D

class_name LevelTemplate

@export var next_level: PackedScene = null

@onready var robot: Robot = $Robot
@onready var garbage: Node2D = $Garbage
@onready var charger: Charger = $Charger

func _ready() -> void:
	charger.level_complete.connect(_on_level_complete)
	robot.captured.connect(_on_robot_captured)

func _process(_delta: float) -> void:
	GlobalState.garbage_left = garbage.get_child_count()

	if Input.is_action_just_pressed("restart"):
		TransitionLayer.reload_current_scene()


func _on_level_complete() -> void:
	print("Level complete!")
	if next_level != null:
		TransitionLayer.load_scene(next_level)

func _on_robot_captured() -> void:
	TransitionLayer.reload_current_scene()
