using Godot;
using System;

/// <summary>
/// A single discrete reading of an input event, capturing its value,
/// modifier key state, and originating action. Samples are collected
/// per-frame and processed in order by the bound <see cref="InputAction"/>.
/// </summary>
public struct InputSample {
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

  public static InputSample? From(StringName actionName, InputEvent source) {
    Vector3 value;
    switch (source) {
      case InputEventKey key:
        value = new(key.Pressed ? 1f : 0f, 0, 0);
        break;
      case InputEventMouseButton mb:
        value = new(mb.Pressed ? 1f : 0f, 0, 0);
        break;
      case InputEventMouseMotion mm:
        value = new(mm.Relative.X, mm.Relative.Y, 0);
        break;
      case InputEventJoypadButton jb:
        value = new(jb.Pressed ? 1f : 0f, 0, 0);
        break;
      case InputEventJoypadMotion axis:
        value = new(axis.AxisValue, 0, 0);
        break;
      default:
        GD.PushWarning($"Unsupported InputEvent type {source.GetType().Name} for action '{actionName}'.");
        return null;
    }

    return new InputSample { ActionName = actionName, Value = value, Keys = ExtractModifierKeys(source) };
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