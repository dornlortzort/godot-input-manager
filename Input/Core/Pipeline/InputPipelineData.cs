using Godot;
using System;

public struct InputPipelineData {
  public Vector3 Value;
  public ModifierKeys Keys;
  public StringName ActionName { get; init; }
  //other metadata useful for debugging

  public bool AsBool() => Value.X > 0.5f;
  public float AsFloat() => Value.X;
  public Vector2 AsVector2() => new(Value.X, Value.Y);
  public Vector3 AsVector3() => Value;

  [Flags]
  public enum ModifierKeys : byte {
    None = 0,
    Shift = 1 << 0,
    Ctrl = 1 << 1,
    Alt = 1 << 2,
    Meta = 1 << 3,
  }

  private static InputPipelineData Create(StringName actionName, InputEvent source) {
    var keys = ExtractModifierKeys(source);
    Vector3 value = source switch {
      InputEventKey key => new(key.Pressed ? 1f : 0f, 0, 0),
      InputEventMouseButton mb => new(mb.Pressed ? 1f : 0f, 0, 0),
      InputEventMouseMotion mm => new(mm.Relative.X, mm.Relative.Y, 0),
      InputEventJoypadButton jb => new(jb.Pressed ? 1f : 0f, 0, 0),
      InputEventJoypadMotion axis => new(axis.AxisValue, 0, 0),
      _ => Vector3.Zero
    };
    return new InputPipelineData { ActionName = actionName, Value = value, Keys = keys };
  }

  private static ModifierKeys ExtractModifierKeys(InputEvent e) {
    var keys = ModifierKeys.None;
    if (e is not InputEventWithModifiers withMods) return keys;

    if (withMods.ShiftPressed) keys |= ModifierKeys.Shift;
    if (withMods.CtrlPressed) keys |= ModifierKeys.Ctrl;
    if (withMods.AltPressed) keys |= ModifierKeys.Alt;
    if (withMods.MetaPressed) keys |= ModifierKeys.Meta;

    return keys;
  }
}