using Godot;

public static class ModifierUtils {
  public static Variant PassUnsupportedValue(Variant val, InputModifierDebugContext ctx, string nameOfModifier) {
    GD.PushError(
      $"{nameOfModifier} on action '{ctx.ActionName}' has an incompatible input type for this operation: '{val.VariantType}'. Passing through unchanged. You should probably remove this modifier from the action.");
    return val;
  }
}