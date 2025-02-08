class_name Cat

extends Node2D

@export var _move_speed: float = 400

@onready var _sprite: Sprite2D = $Sprite2D
@onready var _robot_capture_area: Area2D = $RobotCaptureArea

var _path_follow: PathFollow2D

# needs to remember the previous position to flip the sprite
# instead of using PathFollow2D's rotation
# because it rotates the Cat in x and y axis,
# but we only need to flip the sprite in x axis
var _prev_global_position: Vector2
var _robot_target: Robot
var _tween_capture: Tween


func _ready() -> void:
	_path_follow = get_parent() as PathFollow2D
	if _path_follow == null:
		print_debug("Cat(%s) needs to be a child of PathFollow2D" % name)

	_prev_global_position = global_position

	_robot_capture_area.body_entered.connect(_robot_capture_area_on_body_entered)
	_robot_capture_area.body_exited.connect(_robot_capture_area_on_body_exited)

	GameEvents.robot_can_be_captured_by_enemy.connect(_on_robot_can_be_captured_by_enemy)


func _process(delta: float) -> void:
	if _path_follow != null:
		_path_follow.progress += _move_speed * delta

		# flip the Sprite2D in x axis
		# if the Cat is moving to the left
		const EPSILON: float = 1
		_sprite.flip_h = _prev_global_position.x > global_position.x + EPSILON
	_prev_global_position = global_position


func _robot_capture_area_on_body_entered(body: Node2D) -> void:
	if body is not Robot:
		return

	_robot_target = body as Robot
	if _robot_target.get_can_be_captured_by_enemy():
		_capture_robot()
		return


func _robot_capture_area_on_body_exited(body: Node2D) -> void:
	if body is not Robot:
		return
	_robot_target = null


func _on_robot_can_be_captured_by_enemy(can: bool) -> void:
	if can and _robot_target != null:
		_capture_robot()


func _capture_robot() -> void:
	assert(_robot_target != null, "make sure _robot_target is not null here")

	if _tween_capture != null:
		# tween is running. the Cat is moving to the robot
		return

	# because tween uses the Robot global position to move the Cat
	# and we don't want to change Robot position
	_robot_target.make_not_moveable()

	# stop following the path
	# in order to move to the robot
	_path_follow = null

	# rotate the Cat to the robot
	_sprite.flip_h = global_position.x > _robot_target.global_position.x

	const TWEEN_DURATION: float = .2
	_tween_capture = create_tween()
	_tween_capture.finished.connect(_destroy_robot.call_deferred)
	_tween_capture.tween_property(
		self, "global_position", _robot_target.global_position, TWEEN_DURATION
	)


func _destroy_robot() -> void:
	_robot_target.mark_as_captured_by_enemy(self)
	_robot_target = null
