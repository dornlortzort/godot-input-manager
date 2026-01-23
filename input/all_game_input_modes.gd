@tool
class_name AllGameInputModes
extends Resource

const RESOURCE_PATH := "res://input/modes/all_modes.tres" # Save your resource here

@export var _modes: Array[InputMode]

@export_tool_button("Reload", "Callable") var reload_button = reload
func reload() -> AllGameInputModes:
    _instance = null
    return get_instance()

# Static cached instance
static var _instance: AllGameInputModes

static func get_instance() -> AllGameInputModes:
    if _instance == null or not is_instance_valid(_instance):
        if ResourceLoader.exists(RESOURCE_PATH):
            _instance = load(RESOURCE_PATH)
        else:
            push_warning("AllGameInputModes not found at: %s" % RESOURCE_PATH)
    return _instance


static func get_instance_modes() -> Array[InputMode]:
    return AllGameInputModes.get_instance()._modes

func get_mode(mode_name: StringName) -> InputMode:
    for mode in _modes:
        if mode.mode_name == mode_name:
            return mode
    return null

func get_all_action_names() -> Array[StringName]:
    var names: Array[StringName] = []
    for mode in _modes:
        names.append_array(mode.get_all_names())
    return names