extends StaticBody2D

class_name Bed

var color_inside = Color(1, 1, 1, 0.5)
var color_outside = Color(1, 1, 1, 1)

@onready var _area: Area2D = $"Area2D"
@onready var _sprite: Sprite2D = $"Sprite2D"

func _ready() -> void:
	_area.body_entered.connect(_on_body_entered)
	_area.body_exited.connect(_on_body_exited)

func _on_body_entered(body: Node2D) -> void:
	if (body as Robot) == null:
		return
	var tween := create_tween().set_trans(Tween.TRANS_QUAD)
	tween.tween_property(_sprite, "modulate", color_inside, 0.1)
	body.invisible = true

func _on_body_exited(body: Node2D) -> void:
	if (body as Robot) == null:
		return
	var tween := create_tween().set_trans(Tween.TRANS_QUAD)
	tween.tween_property(_sprite, "modulate", color_outside, 0.1)
	body.invisible = false
