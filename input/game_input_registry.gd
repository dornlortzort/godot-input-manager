@tool
class_name GameInputRegistry
extends Resource

const RESOURCE_PATH := "res://input/game/registry.tres" # Save your resource here

@export var _modes: Array[GameInputMode]

@export_tool_button("Reload", "Callable") var reload_button = reload
func reload() -> GameInputRegistry:
    _instance = null
    return get_instance()

# Static cached instance
static var _instance: GameInputRegistry

static func get_instance() -> GameInputRegistry:
    if _instance == null or not is_instance_valid(_instance):
        if ResourceLoader.exists(RESOURCE_PATH):
            _instance = load(RESOURCE_PATH)
            print_debug("loaded GameInputRegistry successfully")
        else:
            push_warning("GameInputRegistry not found at: %s" % RESOURCE_PATH)
    return _instance


static func get_instance_modes() -> Array[GameInputMode]:
    return GameInputRegistry.get_instance()._modes

func get_mode(mode_name: StringName) -> GameInputMode:
    for mode in _modes:
        if mode.mode_name == mode_name:
            return mode
    return null

func get_all_action_names() -> Array[StringName]:
    var names: Array[StringName] = []
    for mode in _modes:
        names.append_array(mode.get_all_names())
    return names