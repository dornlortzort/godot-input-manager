using Godot;

public partial class ScaleModifier : InputModifier {
  [Export] public float Scale { get; private set; }

  public override Variant Process(Variant input, float delta, in InputDebugContext ctx) {
    return input.VariantType switch {
      Variant.Type.Vector2 => input.As<Vector2>() * Scale,
      Variant.Type.Float => input.As<float>() * Scale,
      _ => InputUtils.Modifier_PassUnsupportedValue(input, ctx, "ScaleModifier")
    };
  }
}