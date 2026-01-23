@tool
class_name InputProfile
extends Resource

@export var profile_name: StringName = &"Default Profile"
@export var bindings: Dictionary[StringName, InputBinding]

@export_category("Validation")
@warning_ignore("unused_private_class_variable")
@export_tool_button("Auto-Populate Missing Actions", "Add") var _populate_button = _auto_populate_missing
func _auto_populate_missing() -> void:
	var added: Array[StringName] = []
	for mode in AllGameInputModes.get_instance_modes():
		for registry_action_name in mode.get_all_names():
			if not bindings.has(registry_action_name):
				var new_binding := InputBinding.new()
				new_binding.event = InputEventKey.new()
				bindings[registry_action_name] = new_binding
				added.append(registry_action_name)
			else:
				push_warning("❌ Found duplicate input action: '%s'" % registry_action_name)

	if added.is_empty():
		print_rich("[color=green]✅ Profile '%s' is already complete.[/color]" % profile_name)
	else:
		print_rich("[color=cyan]➕ Added %d bindings to profile '%s': %s[/color]" % [
			added.size(), profile_name, ", ".join(added)
		])
	
	emit_changed()
	notify_property_list_changed()
	
@warning_ignore("unused_private_class_variable")
@export_tool_button("Validate InputProfile", "Callable") var _validator_button = validate_and_print

func validate_and_print() -> void:
	var missing: Array[StringName]
	var unbound: Array[StringName]
	var kbm_count := 0
	var gamepad_count := 0

	for mode in AllGameInputModes.get_instance_modes():
		for registry_action_name in mode.get_all_names():
			if not bindings.has(registry_action_name):
				missing.append(registry_action_name)
			else:
				var type = bindings[registry_action_name].get_input_type()
				match type:
					InputBinding.InputType.EMPTY, InputBinding.InputType.UNSUPPORTED:
						unbound.append(registry_action_name)
					InputBinding.InputType.KBM:
						kbm_count += 1
					InputBinding.InputType.GAMEPAD:
						gamepad_count += 1
			
	var issues: Array[String] = []
	if not missing.is_empty():
		issues.append("Missing %d actions: %s" % [missing.size(), ", ".join(missing)])
	if not unbound.is_empty():
		issues.append("Unbound events: %s" % ", ".join(unbound))
	if kbm_count > 0 and gamepad_count > 0:
		issues.append("Mixed input types - KBM (%d) | Gamepad (%d)" % [kbm_count, gamepad_count])
	
	if issues.is_empty():
		var type_name := "KBM" if kbm_count > 0 else ("Gamepad" if gamepad_count > 0 else "None")
		print_rich("[color=green]✅ Profile '%s': All %d actions bound (%s)[/color]" % [
			profile_name, bindings.size(), type_name
		])
	else:
		for issue in issues:
			push_warning("❌ Profile '%s': %s" % [profile_name, issue])
