using Godot;

public enum InputEventType : byte {
  Unknown,
  Key,
  MouseButton,
  MouseMotion,
  JoypadButton,
  JoypadMotion,
}

public readonly record struct InputEventLookupKey(InputEventType EventType, int Identifier) {
  public static InputEventLookupKey From(InputEvent e) => e switch {
    InputEventKey key => new(InputEventType.Key, (int)key.Keycode),
    InputEventMouseButton mb => new(InputEventType.MouseButton, (int)mb.ButtonIndex),
    InputEventMouseMotion mm => new(InputEventType.MouseMotion, 0),
    InputEventJoypadButton jb => new(InputEventType.JoypadButton, (int)jb.ButtonIndex),
    //todo: add a way to distinguish AxisValue (determines direction) since godot doesn't have a singular
    // encompassing axis InputEventJoypadMotion. In other words, X-axis's "left" and "right" events are
    // treated as two separate motion events. Could technically just use one event (since both "left" and
    // "right" can be used to report the actual underlying value, but it'd look weird without better editor
    // ui to signal that.
    InputEventJoypadMotion jm => new(InputEventType.JoypadMotion, (int)jm.Axis),
    _ => new(InputEventType.Unknown, 0)
  };
}