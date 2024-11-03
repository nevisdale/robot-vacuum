extends Node2D

class_name Cat

@export var _move_speed: float = 400

@onready var _robot_capture_area: Area2D = $RobotCaptureArea
@onready var _sprite: Sprite2D = $Sprite2D

var _path_follow: PathFollow2D = null

# needs to remember the previous position to flip the sprite
# instead of using PathFollow2D's rotation
# because it rotates the Cat in x and y axis,
# but we only need to flip the sprite in x axis
var _prev_global_position: Vector2 = Vector2.ZERO


func _ready() -> void:
	if get_parent() is PathFollow2D:
		_path_follow = get_parent() as PathFollow2D
	else:
		print_debug(self, " does not have a PathFollow2D parent. do not follow the path.")

	_robot_capture_area.body_entered.connect(_try_capture_robot)
	_prev_global_position = global_position

func _process(delta: float) -> void:
	if _path_follow != null:
		_path_follow.progress += _move_speed * delta
		# flip the Sprite2D in x axis
		# if the Cat is moving to the left
		_sprite.flip_h = _prev_global_position.x > global_position.x
	_prev_global_position = global_position

func _try_capture_robot(body: Node2D) -> void:
	var robot := body as Robot
	assert(robot != null, "The body is not a Robot.")

	if not robot.can_be_captured():
		return

	# stop following the path
	# in order to move to the robot's position
	_path_follow = null

	# rotate the Cat to face the robot
	_sprite.flip_h = global_position > robot.global_position

	# cat remembers the robot's position and then moves there.
	# don't allow the robot to move,
	# otherwise in the end the cat will be outside the robot's position.
	robot.make_not_movable()

	const jump_duration := 0.2
	var tween := create_tween()
	tween.finished.connect(robot.make_captured)
	tween.tween_property(self, "global_position", robot.global_position, jump_duration)
