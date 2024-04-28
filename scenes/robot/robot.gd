class_name Robot

extends CharacterBody2D

@export var _friction: float = 1200
@export var _accel: float = 1200
@export var _max_move_speed: float = 1200

func _physics_process(delta: float) -> void:
	_move(delta)

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
