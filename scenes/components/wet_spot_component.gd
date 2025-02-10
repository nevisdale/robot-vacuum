class_name WetSpotComponent

extends Node2D

@export var spot_area: Area2D

# prevent playing sound for objects that are inside the area when the game starts
var _can_play_sound: bool = false


func _ready() -> void:
	assert(spot_area != null, "spot_area must be set in editor")

	_set_can_play_sound_after(.1)

	spot_area.body_entered.connect(_on_body_entered)
	spot_area.body_exited.connect(_on_body_exited)


func _set_can_play_sound_after(seconds: float) -> void:
	await get_tree().create_timer(seconds).timeout
	_can_play_sound = true


func _on_body_entered(body: Node2D) -> void:
	var garbage := body as PhysicsGarbageComponent
	if garbage == null:
		return
	var applied := garbage.add_wet_spot()
	if applied and _can_play_sound:
		AudioManager.play_sound_wet_spot_affection()


func _on_body_exited(body: Node2D) -> void:
	var garbage := body as PhysicsGarbageComponent
	if garbage != null:
		garbage.remove_wet_spot()
