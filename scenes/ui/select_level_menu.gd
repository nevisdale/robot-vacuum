class_name SelectLevelMenu

extends CanvasLayer

@export var _select_level_button: PackedScene

@onready var _grid_container: GridContainer = %GridContainer
@onready var _back_button: Button = %BackButton


func _ready() -> void:
	OptionsManager.enable_mouse()
	AudioManager.register_buttons([_back_button])

	var all_levels := LevelManager.get_all_level_definitions()
	_create_select_buttons(all_levels)

	_back_button.pressed.connect(_on_back_button_pressed)


func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("pause"):
		LevelManager.go_to_main_menu(false)


func _create_select_buttons(level_definitions: Array[LevelDefinition]) -> void:
	for i: int in range(len(level_definitions)):
		var level_definition := level_definitions[i]
		if level_definition.is_intro:
			continue

		var button := _select_level_button.instantiate() as LevelButton
		_grid_container.add_child(button)
		button.set_title(str(i))
		if not SaveManager.get_can_choose_level(level_definition.level_id):
			button.set_disabled(true)

		AudioManager.register_buttons([button])

		button.level_button_pressed.connect(
			func() -> void: LevelManager.load_level(level_definition.level_id)
		)


func _on_back_button_pressed() -> void:
	LevelManager.go_to_main_menu(false)
