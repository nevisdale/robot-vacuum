class_name PhysicsGarbageComponent

extends RigidBody2D

@onready var _collision_shape: CollisionShape2D = $CollisionShape2D

var _tocuhed_by_robot_last_time: float = 0
var _touched_by_car_last_time: float = 0
var _wet_spot_count: int = 0
var _init_damp: Vector2 # linear damp and angular damp
var _wet_damp: Vector2 = Vector2.ZERO


func _ready() -> void:
	_init_damp = Vector2(linear_damp, angular_damp)


func can_be_captured_by_bin() -> bool:
	return true


func can_be_captured_by_robot() -> bool:
	return false


func capture(capturer: Node2D) -> void:
	const TWEEN_DURATION = .3
	const SCALE_FACTOR = .2

	var tween := create_tween()
	tween.set_parallel(true)
	tween.tween_property(self, "global_position", capturer.global_position, TWEEN_DURATION)
	tween.tween_property(self, "scale", scale * SCALE_FACTOR, TWEEN_DURATION)

	tween.finished.connect(owner.queue_free)

	# strange behavior:
	# when you push the physics garbage towards the trash,
	# the robot is pushed out when the the garbage disappears.
	_collision_shape.set_deferred("disabled", true)


# push applies a force to the physics garbage.
# pass pushForce multiplied by delta to make the force frame rate independent.
func push(kinematic_collision: KinematicCollision2D, push_force: float) -> void:
	var force := push_force * -kinematic_collision.get_normal()
	var pos := kinematic_collision.get_position() - global_position
	apply_force(force, pos)


func touched_by_car() -> void:
	_touched_by_car_last_time = Time.get_ticks_msec()


func is_moving_by_car() -> bool:
	const EPSILON_MS = 10
	return Time.get_ticks_msec() - _touched_by_car_last_time < EPSILON_MS


func touched_by_robot() -> void:
	_tocuhed_by_robot_last_time = Time.get_ticks_msec()


func is_moving_by_robot() -> bool:
	const EPSILON_MS = 300
	return Time.get_ticks_msec() - _tocuhed_by_robot_last_time < EPSILON_MS


func add_wet_spot() -> bool:
	_wet_spot_count += 1
	_update_damp()
	return _wet_spot_count == 1


func remove_wet_spot() -> void:
	_wet_spot_count -= 1
	_update_damp()


func _update_damp() -> void:
	if _wet_spot_count == 0:
		linear_damp = _init_damp.x
		angular_damp = _init_damp.y
	else:
		linear_damp = _wet_damp.x
		angular_damp = _wet_damp.y
