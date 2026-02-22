using System;
using Godot;

[GlobalClass]
public partial class ResponseCurveModifier : InputModifier {
  [Export] public Curve Curve { get; private set; } = new();


  [Obsolete("Use parameterized constructor")]
  public ResponseCurveModifier() {
  }

  public ResponseCurveModifier(Curve curve) {
    Curve = curve;
  }

  public override InputSample Process(InputSample input) {
    if (input.Value.Length() > 1f)
      GD.PushWarning(
        $"ResponseCurveModifier: input {input.Value} outside expected [0,1] range on '{input.ActionName}'. This may lead to unexpected behavior. Is modifier order correct?");

    input.Value.X = ApplyCurve(input.Value.X);
    input.Value.Y = ApplyCurve(input.Value.Y);
    input.Value.Z = ApplyCurve(input.Value.Z);
    return input;
  }

  private float ApplyCurve(float f) => Mathf.Sign(f) * Curve.Sample(Mathf.Abs(f));
}