class_name Robot

extends CharacterBody2D

const LIGHT_ENEGRY_MIN: float = 1
const LIGHT_ENEGRY_MAX: float = 15

@export var _move_speed: float = 1
@export var _rotation_speed: float = 1
@export var _push_force: float = 5000

@onready var _garbage_capture_area: Area2D = $GarbageCaptureArea
@onready var _point_light_red: PointLight2D = $PointLightRed
@onready var _point_light_green: PointLight2D = $PointLightGreen
@onready var _electricity_receiver: ElectricityReceiverComponent = $ElectricityReceiverComponent

var _danger_area_count: int = 0
var _can_be_captured_by_enemy: bool = true
var _tween_active_light: Tween

var _rotate_left_action: String = "rotate_left"
var _rotate_right_action: String = "rotate_right"
var _move_forward_action: String = "move_forward"
var _all_actions: Array[String] = [_rotate_left_action, _rotate_right_action, _move_forward_action]


func _ready() -> void:
	seed(Time.get_ticks_msec())
	_garbage_capture_area.area_entered.connect(_on_garbage_capture_area_body_entered)
	_electricity_receiver.received_electricity.connect(_on_received_electricity)

	_point_light_red.energy = LIGHT_ENEGRY_MIN
	_point_light_green.energy = LIGHT_ENEGRY_MAX


func _physics_process(delta: float) -> void:
	# rotate robot
	var rotate_dir: float = 0
	if Input.is_action_pressed(_rotate_left_action):
		rotate_dir = -1
	if Input.is_action_pressed(_rotate_right_action):
		rotate_dir = 1
	if rotate_dir != 0:
		rotate(rotate_dir * _rotation_speed * delta)

	# move robot
	velocity = Vector2.ZERO
	if Input.is_action_pressed(_move_forward_action):
		velocity = Vector2.UP.rotated(rotation) * _move_speed
	move_and_slide()

	# push physics garbage
	for i in range(get_slide_collision_count()):
		var kinematic_collision := get_slide_collision(i)
		var garbage := kinematic_collision.get_collider() as PhysicsGarbageComponent
		if garbage == null:
			continue
		garbage.push(kinematic_collision, _push_force * delta)
		if not garbage.is_moving_by_robot():
			AudioManager.play_sound_robot_push_garbage()
		garbage.touched_by_robot()


func mark_as_captured_by_enemy(enemy: Node) -> void:
	if enemy is Cat:
		AudioManager.play_sound_cat_capture_robot()
	elif enemy is Car:
		AudioManager.play_sound_car_capture_robot()

	GameEvents.robot_captured_by_enemy.emit(enemy)


func set_can_be_captured_by_enemy(can: bool) -> void:
	if _can_be_captured_by_enemy != can:
		_can_be_captured_by_enemy = can
		GameEvents.robot_can_be_captured_by_enemy.emit(can)


func get_can_be_captured_by_enemy() -> bool:
	return _can_be_captured_by_enemy


func make_not_moveable() -> void:
	_move_speed = 0
	_rotation_speed = 0


func in_danger_area() -> void:
	_danger_area_count += 1
	if _danger_area_count == 1:
		_in_danger_area_animation()


func out_danger_area() -> void:
	_danger_area_count -= 1
	if _danger_area_count == 0:
		_out_danger_area_animation()


func disable_light() -> void:
	if _tween_active_light != null and _tween_active_light.is_valid():
		_tween_active_light.kill()
	_point_light_red.energy = 0
	_point_light_green.energy = 0


func _on_garbage_capture_area_body_entered(area: Area2D) -> void:
	var garbage: GarbageComponent = _get_garbage_component_from(area)
	if garbage != null and garbage.can_be_captured_by_robot():
		garbage.capture(self)
		AudioManager.play_sound_robot_capture_garbage()


func _get_garbage_component_from(node: Node) -> GarbageComponent:
	if node is GarbageComponent:
		return node as GarbageComponent
	for child: Node in node.get_children():
		if child is GarbageComponent:
			return child as GarbageComponent
	return null


func _change_control() -> void:
	# make sure that _all_actions is really shuffled
	var first_element := _all_actions[0]
	while first_element == _all_actions[0]:
		_all_actions.shuffle()
	_rotate_left_action = _all_actions[0]
	_rotate_right_action = _all_actions[1]
	_move_forward_action = _all_actions[2]


func _in_danger_area_animation() -> void:
	if _tween_active_light != null and _tween_active_light.is_valid():
		_tween_active_light.kill()

	_tween_active_light = create_tween()
	_tween_active_light.tween_property(_point_light_green, "energy", LIGHT_ENEGRY_MIN, .5)

	# run this tween little bit faster,
	# it shows that the robot is in danger area
	_tween_active_light = create_tween()
	_tween_active_light.set_loops(0)
	_tween_active_light.tween_property(_point_light_red, "energy", LIGHT_ENEGRY_MAX, .2)
	_tween_active_light.tween_property(_point_light_red, "energy", LIGHT_ENEGRY_MIN, .2)


func _out_danger_area_animation() -> void:
	if _tween_active_light != null and _tween_active_light.is_valid():
		_tween_active_light.kill()

	_tween_active_light = create_tween()
	_tween_active_light.tween_property(_point_light_red, "energy", LIGHT_ENEGRY_MIN, .5)
	_tween_active_light.tween_property(_point_light_green, "energy", LIGHT_ENEGRY_MAX, .5)


func _on_received_electricity() -> void:
	_change_control()
	GameEvents.robot_received_electricity.emit()
