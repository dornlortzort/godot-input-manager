@tool
class_name InputBinding
extends Resource
var _event: InputEvent

@export var event: InputEvent:
	set(value):
		if _event and _event.changed.is_connected(_update_resource_name):
			_event.changed.disconnect(_update_resource_name)
		
		_event = value
		
		if _event and not _event.changed.is_connected(_update_resource_name):
			_event.changed.connect(_update_resource_name)
		
		_update_resource_name()
	get:
		return _event
			
# for proper responsivity when changes are made in godot editor
func _update_resource_name() -> void:
	var event_label := _event.as_text() if _event else ""
	# Sync event's resource_name with its text representation
	if _event and _event.resource_name != event_label:
		_event.resource_name = event_label
	
	# Build binding's display name
	var binding_label := event_label if event_label else "empty"
	if resource_name != binding_label:
		resource_name = binding_label

# utils

enum InputType {EMPTY, UNSUPPORTED, KBM, GAMEPAD}
func get_input_type() -> InputType:
	if _event == null:
		return InputType.EMPTY
	elif _event is InputEventKey or event is InputEventMouseButton or event is InputEventMouseMotion:
		return InputType.KBM
	elif event is InputEventJoypadButton or event is InputEventJoypadMotion:
		return InputType.GAMEPAD
	else:
		return InputType.UNSUPPORTED