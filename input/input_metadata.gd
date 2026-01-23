@tool
class_name InputMetadata
extends Resource

@export var action_name: StringName:
    set(val):
        action_name = val
        _update_resource_name()
@export var is_remappable: bool = true:
    set(val):
        is_remappable = val
        _update_resource_name()
@export var buffer_ms: float = 0:
    set(val):
        buffer_ms = val
        _update_resource_name()

func _init(p_name: StringName = &"", p_is_remappable: bool = true, p_buffer_ms: float = 0) -> void:
    action_name = p_name
    is_remappable = p_is_remappable
    buffer_ms = p_buffer_ms

func _update_resource_name() -> void:
    var label := String(action_name) if action_name else "empty"

    var flags: PackedStringArray = []
    if is_remappable:
        flags.append("R")
    if buffer_ms > 0:
        flags.append("%dms" % buffer_ms)
    
    if flags.size() > 0:
        label += " [%s]" % ", ".join(flags)

    if resource_name != label:
        resource_name = label