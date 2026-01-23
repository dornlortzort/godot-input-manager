@tool
class_name GameInputMode
extends Resource

# Status shown in editor (computed, read-only)
var registry_status: String:
    get:
        if _is_in_registry():
            return "✓ In registry.tres"
        return "⚠ Not in registry.tres"

func _is_in_registry() -> bool:
    var registry := GameInputRegistry.get_instance()
    if registry == null:
        return false
    return self in registry._modes

func _validate_property(property: Dictionary) -> void:
    if property.name == "registry_status":
        property.usage = PROPERTY_USAGE_EDITOR | PROPERTY_USAGE_READ_ONLY

    
@export var mode_name: StringName = &"new_input_mode"
@export_category("Actions")
@export var action_name_to_add: StringName = &""
@export_tool_button("Add Input Action", "Add") var add_button = _add_input_action
func _add_input_action() -> void:
    var base_name := action_name_to_add.strip_edges()
    var i := 0
    
    if base_name.is_empty():
        base_name = "new_action"
    
    var action_name := base_name
    while has(action_name):
        i += 1
        action_name = "%s%s" % [base_name, i]
    
    _actions.append(InputMetadata.new(action_name))
    
    notify_property_list_changed()
    emit_changed()

@export var _actions: Array[InputMetadata] = []

# --- API --- 

func get_all() -> Array[InputMetadata]:
    return _actions

func get_all_names() -> Array[String]:
    return _actions.map(func(a): return a.action_name)

func get_remappables() -> Array[InputMetadata]:
    return _actions.filter(func(a): return a.is_remappable)

func get_action(action_name: String) -> InputMetadata:
    for action in _actions:
        if action.action_name == action_name:
            return action
    return null

func has(action_name: String) -> bool:
    return get_action(action_name) != null