extends Node

# gdscript does not support interfaces,
# so that we use this class to manage it in only one place

class_name Garbage

static func is_garbage(node: Node2D) -> bool:
	# each garbage must have this method
	# to be identified as garbage
	return node.has_method("is_garbage")

static func can_be_captured_by_robot(garbage: Node2D) -> bool:
	return garbage.can_be_captured_by_robot()

static func can_be_captured_by_bin(garbage: Node2D) -> bool:
	return garbage.can_be_captured_by_bin()

static func capture(garbage: Node2D, capturer: Node2D) -> void:
	garbage.capture(capturer)
