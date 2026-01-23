@tool
class_name InputMetadata
extends Resource

@export var action_name: StringName:
    set(value):
        action_name = value
        _update_resource_name()
@export var is_remappable: bool = true
@export var buffer_ms: float = 0

func _init(p_name: StringName = &"", p_is_remappable: bool = true, p_buffer_ms: float = 0) -> void:
    action_name = p_name
    is_remappable = p_is_remappable
    buffer_ms = p_buffer_ms

func _update_resource_name() -> void:
    var label := String(action_name) if action_name else "empty"
    if resource_name != label:
        resource_name = label