using System;
using Godot;

public readonly struct InputDebugContext {
  public StringName ActionName { get; init; }
  //future: DeviceId, MappingContext, etc.
}

public static class InputUtils {
  public static Variant Modifier_PassUnsupportedValue(Variant val, InputDebugContext ctx, string nameOfModifier) {
    GD.PushError(
      $"{nameOfModifier} on action '{ctx.ActionName}' has an incompatible input type for this operation: '{val.VariantType}'. Passing through unchanged. You should probably remove this modifier from the action.");
    return val;
  }

  public static InputPhase Trigger_WarnUnsupportedValueThenReturnNone(Variant val, InputDebugContext ctx,
    string nameOfTrigger) {
    GD.PushError(
      $"{nameOfTrigger} on action '{ctx.ActionName}' got an incompatible input type: '{val.VariantType}'. Returning InputPhase.None You should probably remove this modifier from the action.");
    return InputPhase.None;
  }

  public static float GetBoolMagnitude(bool b) {
    return b ? 1.0f : 0.0f;
  }

  public static float GetFloatMagnitude(float f) {
    return MathF.Abs(f);
  }

  public static float GetVector2Magnitude(Vector2 v) {
    return v.Length();
  }
}