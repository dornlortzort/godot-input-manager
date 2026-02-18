using Godot;

//todo: ensure StringName comparison works correctly in the generated GetHashCode.
// it should, but just double-check for sanity.
public readonly record struct InputBindingLookupKey(StringName EventType, int Device, int Identifier) {
  public static InputBindingLookupKey From(InputEvent e) {
    var id = e switch {
      InputEventKey key => (int)key.PhysicalKeycode,
      InputEventMouseButton mb => (int)mb.ButtonIndex,
      InputEventJoypadButton jb => (int)jb.ButtonIndex,
      InputEventJoypadMotion jm => (int)jm.Axis,
      _ => 0
    };
    return new InputBindingLookupKey(e.GetClass(), e.Device, id);
  }
}

/*
// Then your manager builds the lookup once when the input mode activates:

Dictionary<InputSourceKey, List<InputBinding>> _bindingLookup;

void BuildLookup(InputMode mode) {
    _bindingLookup = new();
    foreach (var (action, binding) in mode.AllBindings()) {
        var key = new InputSourceKey(binding.Source);
        if (!_bindingLookup.TryGetValue(key, out var list)) {
            list = new List<InputBinding>();
            _bindingLookup[key] = list;
        }
        list.Add(binding);
    }
}

//And at runtime it's a single dictionary lookup:
//You'd still want to include device in the key if you're supporting local multiplayer with multiple controllers, since two gamepads will both fire JoyAxis.LeftX events.

public override void _Input(InputEvent e) {
       var key = new InputSourceKey(e);
       if (_bindingLookup.TryGetValue(key, out var bindings)) {
           foreach (var binding in bindings) {
               binding.Process(e);
           }
       }
   }
*/