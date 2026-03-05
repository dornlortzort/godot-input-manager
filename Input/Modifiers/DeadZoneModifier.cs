using System;
using Godot;


[Tool]
[GlobalClass]
public partial class DeadZoneModifier : InputModifier {
  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float InnerThreshold { get; private set; } = 0.2f;

  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float OuterThreshold { get; private set; } = 0.95f;


  [Export] public AxisModeEnum Mode { get; private set; } = AxisModeEnum.Axial;


  [Obsolete("Use parameterized constructor")]
  public DeadZoneModifier() {
  }

  public DeadZoneModifier(float innerThreshold, float outerThreshold,
    AxisModeEnum mode = AxisModeEnum.Axial) {
    InnerThreshold = Math.Clamp(0, innerThreshold, 1);
    OuterThreshold = Math.Clamp(innerThreshold, outerThreshold, 1);
    Mode = mode;
  }

  public override InputPayload Process(InputPayload input) {
    return Mode switch {
      AxisModeEnum.Axial => ProcessAxial(input),
      AxisModeEnum.Radial => ProcessRadial(input),
      _ => throw new ArgumentOutOfRangeException()
    };
  }

  private InputPayload ProcessAxial(InputPayload input) {
    return new(
      ApplyDeadZone(input.X),
      ApplyDeadZone(input.Y),
      ApplyDeadZone(input.Z));
  }

  private float? ApplyDeadZone(float? value) {
    if (!value.HasValue) return null;
    if (OuterThreshold <= InnerThreshold) return 0f;
    var abs = Math.Abs(value.Value);
    if (abs < InnerThreshold) return 0f;
    var remapped = Mathf.Clamp(
      Mathf.InverseLerp(InnerThreshold, OuterThreshold, abs), 0f, 1f);
    return Math.Sign(value.Value) * remapped;
  }

  private InputPayload ProcessRadial(InputPayload input) {
    var magnitude = input.Length();

    if (magnitude < InnerThreshold || OuterThreshold <= InnerThreshold)
      return new(
        input.X.HasValue ? 0f : null,
        input.Y.HasValue ? 0f : null,
        input.Z.HasValue ? 0f : null);

    var remapped = Mathf.Clamp(
      Mathf.InverseLerp(InnerThreshold, OuterThreshold, magnitude), 0f, 1f);
    var scaled = input.Normalized() * remapped;

    return new(
      input.X.HasValue ? scaled.X : null,
      input.Y.HasValue ? scaled.Y : null,
      input.Z.HasValue ? scaled.Z : null);
  }
}

/*
Notes:
Lerp = (t) => a value between a and b
so, InverseLerp = (a value between a and b) => t

Meaning this function will proportionalize the provided input as if
the lower and upper bound are 0 and 1.
*/