class_name ElectricityReceiverComponent

extends Node2D

signal received_electricity


func receive_electricity() -> void:
	AudioManager.play_sound_get_electricity()
	received_electricity.emit()
