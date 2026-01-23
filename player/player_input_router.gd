class_name PlayerInputRouter
extends Node

@export var movement_state_machine: MovementStateMachine
@export var ability_manager: AbilityManager

func _ready():
	InputManager.jump_pressed.connect(_on_jump)

func _on_jump():
	print("jump")
	
	
