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
    InputEventJoypadMotion jm => new(InputEventType.JoypadMotion, (int)jm.Axis),
    _ => new(InputEventType.Unknown, 0)
  };
}