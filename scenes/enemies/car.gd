class_name Car

extends CharacterBody2D

@export var _move_speed: float = 800
@export var _push_force: float = 4000
@export var _use_raycast: bool = true
@export var _wheel_rotation_speed: float = 5

@onready var _ray_casts_left: Node2D = $RayCastLeft
@onready var _ray_casts_right: Node2D = $RayCastRight
@onready var _wheel_sprites: Node2D = $Wheels
@onready var _electricity_visual: AnimatedSprite2D = $ElectricityVisual
@onready var _electricity_receiver: ElectricityReceiverComponent = $ElectricityReceiverComponent

var _direction: Vector2 = Vector2.ZERO
var _wheels: Array[Sprite2D]
var _has_electricity: bool = false


func _ready() -> void:
	_update_electricity_visual()

	for child: Node in _wheel_sprites.get_children():
		if child is Sprite2D:
			_wheels.append(child as Sprite2D)

	GameEvents.floor_button_left_just_pressed.connect(_on_floor_button_left_just_pressed)
	GameEvents.floor_button_right_just_pressed.connect(_on_floor_button_right_just_pressed)
	_electricity_receiver.received_electricity.connect(_on_received_electricity)


func _physics_process(delta: float) -> void:
	# if the car is not moving, then do nothing
	# otherwise, robot can move a car using physics garbage
	if _direction == Vector2.ZERO:
		_try_push_garbage(delta)
		return

	var global_position_before := global_position
	var collision := move_and_collide(_direction * _move_speed * delta)
	if collision != null:
		var collider := collision.get_collider()

		# push garbage
		var garbage := collider as PhysicsGarbageComponent
		if garbage != null:
			garbage.push(collision, _push_force * delta)
			garbage.touched_by_car()

		# capture robot
		var robot := collider as Robot
		if robot != null:
			_direction = Vector2.ZERO
			_wheel_rotation_speed = 0
			robot.make_not_moveable()
			robot.mark_as_captured_by_enemy(self)

		# walls or other static object, just stop a car
		if collider is StaticBody2D or collider is TileMapLayer:
			_direction = Vector2.ZERO
			_wheel_rotation_speed = 0

		# is car stuck?
		if global_position_before.is_equal_approx(global_position):
			_try_push_garbage(delta)

	for sprite: Sprite2D in _wheels:
		sprite.rotation += _direction.x * _wheel_rotation_speed * delta


func _try_push_garbage(delta: float) -> void:
	if not _use_raycast:
		return
	_try_push_garbage_from(_ray_casts_left, Vector2.LEFT, delta)
	_try_push_garbage_from(_ray_casts_right, Vector2.RIGHT, delta)


func _try_push_garbage_from(group: Node, force_direction: Vector2, delta: float) -> void:
	var child_count := group.get_child_count()
	for i: int in range(child_count):
		var raycast: RayCast2D = group.get_child(i)
		raycast.force_raycast_update()
		if !raycast.is_colliding():
			continue
		var garbage := raycast.get_collider() as PhysicsGarbageComponent
		if garbage == null:
			continue

		const FORCE_FACTOR := .01
		var force := _push_force * force_direction * FORCE_FACTOR * delta
		garbage.apply_central_force(force)


func _on_floor_button_left_just_pressed() -> void:
	var dir := Vector2.LEFT
	if _has_electricity:
		dir = Vector2.RIGHT
	_direction = dir


func _on_floor_button_right_just_pressed() -> void:
	var dir := Vector2.RIGHT
	if _has_electricity:
		dir = Vector2.LEFT
	_direction = dir


func _on_received_electricity() -> void:
	_has_electricity = !_has_electricity
	_direction = -_direction
	_update_electricity_visual()


func _update_electricity_visual() -> void:
	if _has_electricity:
		_electricity_visual.show()
	else:
		_electricity_visual.hide()
