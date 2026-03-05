using System;
using Godot;

/// <summary>
/// A single discrete reading of an input event, capturing its value,
/// modifier key state, and originating action. Payloads are collected
/// across a frame and processed in order by the binding's <see cref="InputAction"/>.
/// </summary>
public readonly struct InputPayload(float? x = null, float? y = null, float? z = null) {
  public readonly float? X = x;
  public readonly float? Y = y;
  public readonly float? Z = z;

  public static InputPayload? From(InputEvent source) {
    return source switch {
      InputEventKey key =>
        new InputPayload(x: key.Pressed ? 1f : 0f),
      InputEventMouseButton mb =>
        new InputPayload(x: mb.Pressed ? 1f : 0f),
      InputEventMouseMotion mm =>
        new InputPayload(x: mm.Relative.X, y: mm.Relative.Y),
      InputEventJoypadButton jb =>
        new InputPayload(x: jb.Pressed ? 1f : 0f),
      InputEventJoypadMotion axis =>
        new InputPayload(x: axis.AxisValue),
      _ => LogUnsupported(source)
    };
  }

  private static InputPayload? LogUnsupported(InputEvent source) {
    GD.PushWarning($"Unsupported InputEvent type {source.GetType().Name}.");
    return null;
  }


  public float Length() {
    var x = X ?? 0f;
    var y = Y ?? 0f;
    var z = Z ?? 0f;
    return MathF.Sqrt(x * x + y * y + z * z);
  }

  /// <summary>
  /// Null division is lifted, so null channels stay null automatically
  /// </summary>
  public InputPayload Normalized() {
    var len = Length();
    if (len == 0f) return this;
    return new(X / len, Y / len, Z / len);
  }

  public override string ToString() =>
    $"({X?.ToString() ?? "null"}, {Y?.ToString() ?? "null"}, {Z?.ToString() ?? "null"})";

  public static InputPayload operator -(InputPayload p) =>
    new(-p.X, -p.Y, -p.Z);

  public static InputPayload operator *(InputPayload p, float scalar) =>
    new(p.X * scalar, p.Y * scalar, p.Z * scalar);
}