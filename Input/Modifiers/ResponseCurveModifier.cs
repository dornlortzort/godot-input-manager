using System;
using Godot;

[Tool]
[GlobalClass]
public partial class ResponseCurveModifier : InputModifier {
  [Export] public Curve Curve { get; private set; } = new();
  [Export] public AxisModeEnum Mode { get; private set; } = AxisModeEnum.Axial;


  [Obsolete("Use parameterized constructor")]
  public ResponseCurveModifier() {
  }

  public ResponseCurveModifier(Curve curve) {
    Curve = curve;
  }

  public override InputPayload Process(InputPayload input) => Mode switch {
    AxisModeEnum.Axial => ProcessAxial(input),
    AxisModeEnum.Radial => ProcessRadial(input),
    _ => throw new ArgumentOutOfRangeException()
  };


  private InputPayload ProcessAxial(InputPayload input) {
    return new(
      ApplyCurve(input.X),
      ApplyCurve(input.Y),
      ApplyCurve(input.Z));
  }

  private float? ApplyCurve(float? value) {
    if (!value.HasValue) return null;
    if (value > 1f)
      GD.PushWarning(
        $"ResponseCurveModifier: input outside expected [0,1] range. Is modifier order correct?");
    return Mathf.Sign(value.Value) * Curve.Sample(Mathf.Abs(value.Value));
  }

  private InputPayload ProcessRadial(InputPayload input) {
    var magnitude = input.Length();
    if (magnitude == 0f) return input;

    if (magnitude > 1f)
      GD.PushWarning(
        "ResponseCurveModifier: magnitude outside expected [0,1] range. Is modifier order correct?");

    var curved = Curve.Sample(magnitude);
    var scaled = input.Normalized() * curved;

    return new(
      input.X.HasValue ? scaled.X : null,
      input.Y.HasValue ? scaled.Y : null,
      input.Z.HasValue ? scaled.Z : null);
  }
}