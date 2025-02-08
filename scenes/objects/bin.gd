class_name Bin

extends StaticBody2D

@onready var _capture_area: Area2D = $GarbageCaptureArea


func _ready() -> void:
	_capture_area.body_entered.connect(_on_capture_area_body_entered)


func _on_capture_area_body_entered(body: Node) -> void:
	# physics body is a child of the node
	# so we need to get the owner of the physics body
	# to get the garbage component
	var garbage: GarbageComponent = _get_garbage_component_from(body.owner)
	if garbage != null and garbage.can_be_captured_by_bin():
		garbage.capture(self)
		AudioManager.play_sound_bin_capture_garbage()


func _get_garbage_component_from(node: Node) -> GarbageComponent:
	if node is GarbageComponent:
		return node as GarbageComponent
	for child: Node in node.get_children():
		if child is GarbageComponent:
			return child as GarbageComponent
	return null
