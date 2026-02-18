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

  public override InputActionPhaseEnum Evaluate(InputPipelineData input, float delta) {
    var magnitude = input.Value.Length();

    var isAbove = magnitude >= Threshold;
    switch (_state) {
      case State.Idle:
        if (!isAbove) return InputActionPhaseEnum.None;

        _elapsed = 0f;
        _state = State.Pending;
        return InputActionPhaseEnum.Pending;
      case State.Pending:
        if (!isAbove) {
          _state = State.Idle;
          return InputActionPhaseEnum.Canceled;
        }

        _elapsed += delta;
        if (_elapsed < Duration) return InputActionPhaseEnum.Pending;

        _state = State.Held;
        return InputActionPhaseEnum.Activated;
      case State.Held:
        if (isAbove) return InputActionPhaseEnum.Activated;

        _state = State.Idle;
        return InputActionPhaseEnum.Completed;
    }

    return InputActionPhaseEnum.None;
  }

  public override void Reset() {
    _elapsed = 0.0f;
    _state = State.Idle;
  }
}