class_name GarbageComponent

extends Node

@export var _extended_garbage_component: Node

var _required_methods := ["can_be_captured_by_bin", "can_be_captured_by_robot", "capture"]


func _ready() -> void:
	assert(
		_extended_garbage_component != null, "_extended_garbage_component must be set in the editor"
	)

	for method: String in _required_methods:
		var method_exists := _extended_garbage_component.has_method(method)
		assert(method_exists, "%s must be implemented in _extended_garbage_component" % method)


func can_be_captured_by_bin() -> bool:
	@warning_ignore("unsafe_method_access")
	return _extended_garbage_component.can_be_captured_by_bin()


func can_be_captured_by_robot() -> bool:
	@warning_ignore("unsafe_method_access")
	return _extended_garbage_component.can_be_captured_by_robot()


func capture(capturer: Node2D) -> void:
	@warning_ignore("unsafe_method_access")
	_extended_garbage_component.capture(capturer)
