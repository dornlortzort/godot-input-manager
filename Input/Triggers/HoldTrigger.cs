using Godot;

/// <summary>
///   HoldTrigger is the same as PressTrigger but you have to meet a SecondsToHold
///   requirement
/// </summary>
public partial class HoldTrigger : InputTrigger {
  private enum State {
    Idle,
    Pending,
    Held
  }

  private State _state;
  private float _elapsed;

  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float Threshold { get; private set; } = 0.3f;

  [Export] public float Duration { get; private set; } = 0.5f;

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
        return InputUtils.Trigger_WarnUnsupportedValueThenReturnNone(value, ctx, "HoldTrigger");
    }

    var isAbove = magnitude >= Threshold;
    switch (_state) {
      case State.Idle:
        if (!isAbove) return InputPhase.None;

        _elapsed = 0f;
        _state = State.Pending;
        return InputPhase.Pending;
      case State.Pending:
        if (!isAbove) {
          _state = State.Idle;
          return InputPhase.Canceled;
        }

        _elapsed += delta;
        if (_elapsed < Duration) return InputPhase.Pending;

        _state = State.Held;
        return InputPhase.Triggered;
      case State.Held:
        if (isAbove) return InputPhase.Sustained;

        _state = State.Idle;
        return InputPhase.Completed;
    }

    return InputPhase.None;
  }

  public override void Reset() {
    _elapsed = 0.0f;
    _state = State.Idle;
  }
}