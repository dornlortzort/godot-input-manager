using System;
using Godot;

[GlobalClass]
public partial class FlickTrigger : InputTrigger {
  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float StartPoint { get; set; } = 0.5f;

  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float EndPoint { get; set; } = 0.95f;

  [Export] public float TimeWindow { get; private set; } = 0.25f;

  public enum FlickAxis {
    Any,
    X,
    Y
  }

  [Export] public FlickAxis RestrictAxis { get; private set; } = FlickAxis.Any;

  /// <summary>
  /// local enums get a bit hairy... when in doubt, just try declaring one inline and see if it compiles.
  /// </summary>
  public override string AsCodeDeclarationString() {
    return
      $"new {nameof(FlickTrigger)}() {{ " +
      $"StartPoint = {StartPoint}f, " +
      $"EndPoint = {EndPoint}f, " +
      $"TimeWindow = {TimeWindow}f, " +
      $"RestrictAxis = {nameof(FlickTrigger)}.{nameof(FlickAxis)}.{RestrictAxis}" +
      $"}}";
  }

  private float _elapsed;
  private bool _isPending;
  private bool _needsReset;


  /// <summary>
  /// Always return activated if it is achieved.
  /// Let all events run without returning early in case the player clears
  /// _needsReset by chance in the same frame they trigger Activated (highly
  /// unlikely this would ever happen, but w/e). 
  /// </summary>
  public override InputActionPhaseEnum Evaluate(ReadOnlySpan<InputSample> samplesThisFrame, float delta) {
    _elapsed += delta;

    var result = InputActionPhaseEnum.None;
    foreach (var sample in samplesThisFrame) {
      var phase = EvaluateSample(sample);
      if (phase != InputActionPhaseEnum.Pending) _elapsed = 0;
      if (result != InputActionPhaseEnum.Activated) result = phase;
    }

    return result;
  }

  protected override InputActionPhaseEnum EvaluateSample(InputSample sample) {
    var magnitude = sample.Value.Length();

    if (_needsReset) {
      if (magnitude < StartPoint) {
        _needsReset = false;
      }

      return InputActionPhaseEnum.None;
    }

    if (!_isPending) {
      if (magnitude < StartPoint) return InputActionPhaseEnum.None;

      // achieved flick in a single frame; no chance to become pending
      if (magnitude >= EndPoint) {
        _needsReset = true;
        return InputActionPhaseEnum.Activated;
      }

      _isPending = true;
      return InputActionPhaseEnum.Pending;
    }

    // in pending state...
    // too slow: cancel
    if (_elapsed > TimeWindow) {
      _isPending = false;
      _needsReset = magnitude >= StartPoint;
      return InputActionPhaseEnum.None;
    }

    // flick achieved:
    if (magnitude >= EndPoint) {
      _isPending = false;
      _needsReset = true;
      return InputActionPhaseEnum.Activated;
    }

    // still pending...
    return InputActionPhaseEnum.Pending;
  }

  public override InputTrigger Clone() {
    var clone = (FlickTrigger)Duplicate();
    clone.Reset();
    return clone;
  }

  public override void Reset() {
    _isPending = false;
    _needsReset = false;
    _elapsed = 0f;
  }
}