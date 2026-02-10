using Godot;

public partial class DownTrigger : InputTrigger {
  [Export] public float Threshold { get; private set; } = 0.3f;
  private bool _isActive;

  public override InputPhase Evaluate(Variant value, float delta, InputDebugContext ctx) {
    float magnitude;
    switch (value.VariantType) {
      case Variant.Type.Bool:
        magnitude = InputUtils.GetBoolMagnitude(value.As<bool>());
        break;
      case Variant.Type.Float:
        magnitude = InputUtils.GetFloatMagnitude(value.As<float>());
        break;
      case Variant.Type.Vector2:
        magnitude = InputUtils.GetVector2Magnitude(value.As<Vector2>());
        break;
      default:
        Reset();
        return InputUtils.Trigger_WarnUnsupportedValueThenReturnNone(value, ctx, "FlickTrigger");
    }

    var isAbove = magnitude >= Threshold;
    switch (isAbove) {
      case true when !_isActive:
        _isActive = true;
        return InputPhase.Triggered;
      case true when _isActive:
        return InputPhase.Sustained;
      case false when _isActive:
        _isActive = false;
        return InputPhase.Completed;
      default:
        return InputPhase.None;
    }
  }

  public override void Reset() {
    _isActive = false;
  }
}