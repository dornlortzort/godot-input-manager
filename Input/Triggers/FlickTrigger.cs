using Godot;

public partial class FlickTrigger : InputTrigger {
  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float StartPoint { get; private set; } = 0.5f;

  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float EndPoint { get; private set; } = 0.95f;

  [Export] public float TimeWindow { get; private set; } = 0.25f;

  public enum FlickAxis {
    Any,
    X,
    Y
  }

  [Export] public FlickAxis RestrictAxis { get; private set; } = FlickAxis.Any;

  private float _elapsed;
  private bool _isPending;
  private bool _needsReset;

  public override InputActionPhaseEnum Evaluate(InputPipelineData input, float delta) {
    var magnitude = input.Value.Length();

    if (_needsReset) {
      if (magnitude < StartPoint) {
        _needsReset = false;
      }

      return InputActionPhaseEnum.None;
    }

    if (!_isPending) {
      // achieved flick in a single frame; no chance to become pending
      if (magnitude >= EndPoint) {
        _needsReset = true;
        return InputActionPhaseEnum.Activated;
      }

      if (magnitude >= StartPoint) {
        _isPending = true;
        _elapsed = 0f;
        return InputActionPhaseEnum.Pending;
      }

      return InputActionPhaseEnum.None;
    }

    _elapsed += delta;

    if (magnitude >= EndPoint) {
      _isPending = false;
      _needsReset = true;
      return InputActionPhaseEnum.Activated;
    }

    if (_elapsed > TimeWindow) {
      _isPending = false;
      _needsReset = magnitude >= StartPoint;
      return InputActionPhaseEnum.Canceled;
    }

    return InputActionPhaseEnum.Pending;
  }

  public override void Reset() {
    _isPending = false;
    _needsReset = false;
    _elapsed = 0f;
  }
}