using System;
using Godot;

/// <summary>
///   Simplest trigger.
///   - Activates on: the frame an input's value exceeds the actuation threshold.
///   - Completes on: the frame an input's value drops back below.
///   Timeline for a single button press:
///   Frame N: value goes above threshold -> Activated
///   Frame N+1: value still above threshold -> Sustained (already fired)
///   Frame N+2: value drops below threshold -> Completed
///   Pending is never used -- this trigger is instantaneous
///   bool -> 1.0 or 0.0
///   float -> MathF.Abs(value)
///   Vector2 -> value.Length()
/// </summary>
public partial class PressTrigger : Resource, IInputTrigger {
  /// <summary>
  ///   Gets set at the end of this frame, processed at the start of the next frame.
  /// </summary>
  private bool flag_activeLastFrame;

  [Export] public float ActuationThreshold { get; set; } = 0.5f;

  public InputPhase Evaluate(Variant value, float delta, InputDebugContext ctx) {
    float magnitude;
    switch (value.VariantType) {
      case Variant.Type.Bool:
        magnitude = 1.0f;
        break;
      case Variant.Type.Float:
        magnitude = GetFloatMagnitude(value.As<float>());
        break;
      case Variant.Type.Vector2:
        magnitude = GetVector2Magnitude(value.As<Vector2>());
        break;
      default:
        flag_activeLastFrame = false;
        return InputUtils.Trigger_WarnUnsupportedValueThenReturnNone(value, ctx, "PressTrigger");
    }

    var isActiveThisFrame = magnitude > ActuationThreshold;
    InputPhase result;
    if (flag_activeLastFrame)
      result = isActiveThisFrame ? InputPhase.Sustained : InputPhase.Completed;
    else
      result = isActiveThisFrame ? InputPhase.Triggered : InputPhase.None;

    flag_activeLastFrame = isActiveThisFrame;
    return result;
  }

  public void Reset() {
    flag_activeLastFrame = false;
  }

  private float GetFloatMagnitude(float f) {
    return MathF.Abs(f);
  }

  private float GetVector2Magnitude(Vector2 v) {
    return v.Length();
  }
}