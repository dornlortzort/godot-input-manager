using Godot;

public partial class ResponseCurveModifier : Resource, IInputModifier {
  [Export] public Curve Curve { get; private set; } = new();

  public Variant Process(Variant input, float delta, in InputModifierDebugContext ctx) {
    return input.VariantType switch {
      Variant.Type.Vector2 => ProcessVector2D(input.As<Vector2>(), ctx),
      Variant.Type.Float => ProcessFloat(input.As<float>(), ctx),
      _ => ModifierUtils.PassUnsupportedValue(input, ctx, "ResponseCurveModifier")
    };
  }

  private float ApplyCurve(float f, InputModifierDebugContext ctx) {
    var abs = Mathf.Abs(f);
    if (abs > 1f)
      GD.PushWarning(
        $"ResponseCurveModifier: input {f} outside expected [0,1] range on '{ctx.ActionName}'. This may lead to unexpected behavior. Is modifier order correct?");

    return Mathf.Sign(f) * Curve.Sample(Mathf.Abs(f));
  }

  private Variant ProcessFloat(float f, InputModifierDebugContext ctx) => Variant.From(ApplyCurve(f, ctx));
  private Variant ProcessVector2D(Vector2 v, InputModifierDebugContext ctx)
    => Variant.From(new Vector2(ApplyCurve(v.X, ctx), ApplyCurve(v.Y, ctx)));
}