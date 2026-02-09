using Godot;

public partial class DeadZoneModifier : Resource, IInputModifier {
  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float InnerThreshold { get; private set; } = 0.2f;

  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float OuterThreshold { get; private set; } = 0.95f;

  public Variant Process(Variant input, float delta, in InputModifierDebugContext ctx) {
    return input.VariantType switch {
      Variant.Type.Vector2 => ProcessVector2(input.As<Vector2>()),
      Variant.Type.Float => ProcessFloat(input.As<float>()),
      _ => ModifierUtils.PassUnsupportedValue(input, ctx, "DeadZoneModifier")
    };
  }

  private Variant ProcessVector2(Vector2 v) {
    var magnitude = v.Length();
    if (magnitude < InnerThreshold) return Variant.From(Vector2.Zero);

    var remappedMagnitude = Mathf.Clamp(Mathf.InverseLerp(InnerThreshold, OuterThreshold, magnitude), 0f, 1f);
    return Variant.From(v.Normalized() * remappedMagnitude);
  }

  private Variant ProcessFloat(float f) {
    float sign = Mathf.Sign(f);
    var abs = Mathf.Abs(f);
    if (abs < InnerThreshold) return Variant.From(0f);

    var remappedMagnitude = Mathf.Clamp(Mathf.InverseLerp(InnerThreshold, OuterThreshold, abs), 0f, 1f);
    return Variant.From(sign * remappedMagnitude);
  }
}

/*
Notes:
Lerp = (t) => a value between a and b
so, InverseLerp = (a value between a and b) => t

Meaning this function will proportionalize the provided input as if
the lower and upper bound are 0 and 1.
*/