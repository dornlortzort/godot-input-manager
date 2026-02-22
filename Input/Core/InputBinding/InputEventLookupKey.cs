using Godot;

public enum InputEventType : byte {
  Unknown,
  Key,
  MouseButton,
  MouseMotion,
  JoypadButton,
  JoypadMotion,
}

public readonly record struct InputEventLookupKey(InputEventType EventType, int Device, int Identifier) {
  public static InputEventLookupKey From(InputEvent e) => e switch {
    InputEventKey key => new(InputEventType.Key, e.Device, (int)key.PhysicalKeycode),
    InputEventMouseButton mb => new(InputEventType.MouseButton, e.Device, (int)mb.ButtonIndex),
    InputEventMouseMotion mb => new(InputEventType.MouseMotion, e.Device, 0),
    InputEventJoypadButton jb => new(InputEventType.JoypadButton, e.Device, (int)jb.ButtonIndex),
    InputEventJoypadMotion jm => new(InputEventType.JoypadMotion, e.Device, (int)jm.Axis),
    _ => new(InputEventType.Unknown, e.Device, 0)
  };
}