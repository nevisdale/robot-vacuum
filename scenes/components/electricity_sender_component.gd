class_name ElectricitySenderComponent
extends Node

signal sent_electricity

@export var _area2d: Area2D
@export var _infinite: bool = false

var _has_electricity: bool = false


func _ready() -> void:
	_area2d.body_entered.connect(_on_body_entered)


func add_eletricity() -> void:
	_has_electricity = true


func _on_body_entered(body: Node) -> void:
	var receiver := _get_electricity_receiver_component_from(body)
	if receiver != null and (_has_electricity or _infinite):
		receiver.receive_electricity()
		sent_electricity.emit()
		_has_electricity = false


func _get_electricity_receiver_component_from(node: Node) -> ElectricityReceiverComponent:
	if node is ElectricityReceiverComponent:
		return node as ElectricityReceiverComponent
	for child: Node in node.get_children():
		if child is ElectricityReceiverComponent:
			return child as ElectricityReceiverComponent
	return null
