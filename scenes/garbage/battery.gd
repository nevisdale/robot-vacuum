class_name Battery

extends Node2D

@onready var _electricity_visual: AnimatedSprite2D = %ElectricityVisual
@onready var _electricity_receiver: ElectricityReceiverComponent = %ElectricityReceiverComponent
@onready var _electricity_sender: ElectricitySenderComponent = %ElectricitySenderComponent


func _ready() -> void:
	_electricity_visual.hide()
	_electricity_receiver.received_electricity.connect(_on_received_electricity)
	_electricity_sender.sent_electricity.connect(_on_sent_eletricity)


func _on_received_electricity() -> void:
	_electricity_visual.show()
	_electricity_sender.add_eletricity()


func _on_sent_eletricity() -> void:
	_electricity_visual.hide()
