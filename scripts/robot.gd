extends CharacterBody2D

class_name Robot

@export var _move_speed: float = 1
@export var _rotation_speed_radian: float = 1
@export var _push_force: float = 5000


@onready var garbage_capture_area: Area2D = $GarbageCaptureArea

func _ready() -> void:
	garbage_capture_area.area_entered.connect(_try_capture_garbage)
	pass

func _physics_process(delta: float) -> void:
	# rotate the robot
	var rorate_dir := 0
	if Input.is_action_pressed("rotate_left"):
		rorate_dir = -1
	elif Input.is_action_pressed("rotate_right"):
		rorate_dir = 1
	if rorate_dir != 0:
		rotate(rorate_dir * _rotation_speed_radian * delta)

	# move the robot
	velocity = Vector2.ZERO
	if Input.is_action_pressed("move_forward"):
		velocity = Vector2.UP.rotated(rotation) * _move_speed
	move_and_slide()

	# push objects
	for i in get_slide_collision_count():
		var kinematic_collision := get_slide_collision(i)
		var rb := kinematic_collision.get_collider() as RigidBody2D
		if rb == null:
			continue
		var force := _push_force * delta * -kinematic_collision.get_normal()
		var pos := kinematic_collision.get_position() - rb.global_position
		rb.apply_force(force, pos)

func _try_capture_garbage(area: Area2D) -> void:
	if not Garbage.is_garbage(area):
		return

	var garbage := area
	var capturer := self
	if Garbage.can_be_captured_by_robot(garbage):
		Garbage.capture(garbage, capturer)
