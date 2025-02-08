extends Camera2D

class_name GameCamera

const NOISE_SAMPLE_GROWTH := .1
const NOISE_FREQUENCY_MULTIPLIER := 150.0
const SHAKE_DECAY := 3
const MAX_CAMERA_OFFSET := 30.0

@export var _shake_noise: FastNoiseLite

var _noise_sample: Vector2
var _current_camera_shake_percentage: float = 0


func shake() -> void:
	_current_camera_shake_percentage = 1


func _process(delta: float) -> void:
	_apply_camera_shake(delta)


func _apply_camera_shake(delta: float) -> void:
	if _current_camera_shake_percentage > 0:
		_noise_sample.x += NOISE_SAMPLE_GROWTH * NOISE_FREQUENCY_MULTIPLIER * delta
		_noise_sample.y += NOISE_SAMPLE_GROWTH * NOISE_FREQUENCY_MULTIPLIER * delta
		_current_camera_shake_percentage = clamp(
			_current_camera_shake_percentage - SHAKE_DECAY * delta, 0, 1
		)

	var x_sample := _shake_noise.get_noise_2d(_noise_sample.x, 0)
	var y_sample := _shake_noise.get_noise_2d(0, _noise_sample.y)

	offset = Vector2(x_sample, y_sample) * MAX_CAMERA_OFFSET * _current_camera_shake_percentage
