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
        var v = value.As<Vector2>();
        magnitude = RestrictAxis switch {
          FlickAxis.X => InputUtils.GetFloatMagnitude(v.X),
          FlickAxis.Y => InputUtils.GetFloatMagnitude(v.Y),
          _ => InputUtils.GetVector2Magnitude(v)
        };
        break;
      default:
        Reset();
        return InputUtils.Trigger_WarnUnsupportedValueThenReturnNone(value, ctx, "FlickTrigger");
    }

    if (_needsReset) {
      if (magnitude < StartPoint) {
        _needsReset = false;
      }

      return InputPhase.None;
    }

    if (!_isPending) {
      // achieved flick in a single frame; no chance to become pending
      if (magnitude >= EndPoint) {
        _needsReset = true;
        return InputPhase.Triggered;
      }

      if (magnitude >= StartPoint) {
        _isPending = true;
        _elapsed = 0f;
        return InputPhase.Pending;
      }

      return InputPhase.None;
    }

    _elapsed += delta;

    if (magnitude >= EndPoint) {
      _isPending = false;
      _needsReset = true;
      return InputPhase.Triggered;
    }

    if (_elapsed > TimeWindow) {
      _isPending = false;
      _needsReset = magnitude >= StartPoint;
      return InputPhase.Canceled;
    }

    return InputPhase.Pending;
  }

  public override void Reset() {
    _isPending = false;
    _needsReset = false;
    _elapsed = 0f;
  }
}