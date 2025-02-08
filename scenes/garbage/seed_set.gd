class_name SeedSet

extends Node2D


func _ready() -> void:
	child_exiting_tree.connect(_on_child_exiting_tree)


func _on_child_exiting_tree(_child: Node) -> void:
	# call deferred to exclude a child from the tree
	#
	# when this signal is received, the child param node is still accessible inside the tree.
	_destroy_if_no_children.call_deferred()


func _destroy_if_no_children() -> void:
	if get_child_count(false) == 0:
		queue_free()
