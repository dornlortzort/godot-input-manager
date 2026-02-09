using Godot;

public partial class ScaleModifier : Resource, IInputModifier {
  [Export] public float Scale { get; private set; }

  public Variant Process(Variant input, float delta, in InputModifierDebugContext ctx) {
    return input.VariantType switch {
      Variant.Type.Vector2 => input.As<Vector2>() * Scale,
      Variant.Type.Float => input.As<float>() * Scale,
      _ => ModifierUtils.PassUnsupportedValue(input, ctx, "ScaleModifier")
    };
  }
}