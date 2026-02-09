using System;
using Godot;

public partial class HoldTrigger : Resource, IInputTrigger {
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

    InputPhase result;
    if (magnitude == 0.0f) {
      result = flag_activeLastFrame ? InputPhase.Canceled : InputPhase.None;
      flag_activeLastFrame = false;
    } else if (magnitude < ActuationThreshold) {
      result = flag_activeLastFrame ? InputPhase.Completed : InputPhase.Pending;
    } else {
      result = InputPhase.Triggered;
      flag_activeLastFrame = true;
    }

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