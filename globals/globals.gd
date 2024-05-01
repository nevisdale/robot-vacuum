extends Node

signal garbage_count_changed(value)
var garbage_count: int = 0:
	get:
		return garbage_count
	set(value):
		garbage_count = value
		garbage_count_changed.emit(value)
