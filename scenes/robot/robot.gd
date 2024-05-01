class_name Robot

extends CharacterBody2D

var invisible = false

@export var _friction: float = 1200
@export var _accel: float = 1200
@export var _max_move_speed: float = 1200

@onready var _capture_area: Area2D = $"CaptureArea"

func _ready():
	_capture_area.area_entered.connect(_on_area_entered)

func _physics_process(delta: float) -> void:
	_move_v2(delta)

func _move(delta: float) -> void:
	var direction := Input.get_vector("move_left", "move_right", "move_up", "move_down")
	if direction == Vector2.ZERO:
		var friction := _friction * delta
		if velocity.length() > friction:
			velocity -= velocity.normalized() * friction
		else:
			velocity = Vector2.ZERO
	else:
		velocity += direction * _accel
		velocity = velocity.limit_length(_max_move_speed)
	move_and_slide()

func _move_v2(_delta: float) -> void:
	if Input.is_key_pressed(KEY_SPACE):
		velocity = Vector2.RIGHT.rotated(rotation) * 500
	else:
		velocity = Vector2.ZERO
	
	if Input.is_action_pressed("move_left"):
		rotation -= 0.1
	if Input.is_action_pressed("move_right"):
		rotation += 0.1

	move_and_slide()

func _on_area_entered(area: Area2D) -> void:
	if area == null:
		return
	if area.is_in_group("Garbage"):
		_capture_garbage(area)
	
func _capture_garbage(garbage: Area2D) -> void:
	var tween := create_tween().set_parallel(true)
	tween.tween_property(garbage, "scale", Vector2.ZERO, 0.1)
	tween.tween_property(garbage, "global_position", global_position, 0.1)
	tween.tween_property(garbage, "rotation", randf_range(0, 2 * PI), 0.1)
	tween.finished.connect(garbage.queue_free)
	Globals.garbage_count -= 1
