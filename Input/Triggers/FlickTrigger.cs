using System;
using Godot;

[Tool]
[GlobalClass]
public partial class FlickTrigger : InputTrigger {
  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float StartPoint { get; set; } = 0.5f;

  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float EndPoint { get; set; } = 0.95f;

  [Export] public float TimeWindow { get; set; } = 0.25f;

  public enum FlickAxis {
    Any,
    X,
    Y,
    Z
  }

  [Export] public FlickAxis RestrictAxis { get; set; } = FlickAxis.Any;

  private double _elapsed;
  private bool _isPending;
  private bool _needsReset;

  public override string AsCodeDeclarationString() {
    return
      $"new {nameof(FlickTrigger)}() {{ " +
      $"StartPoint = {StartPoint}f, " +
      $"EndPoint = {EndPoint}f, " +
      $"TimeWindow = {TimeWindow}f, " +
      // local enums get a bit hairy... when in doubt, just try declaring one inline and see if it compiles.
      $"RestrictAxis = {nameof(FlickTrigger)}.{nameof(FlickAxis)}.{RestrictAxis}" +
      $"}}";
  }

  public override string GetResourceName() => nameof(FlickTrigger).Replace("Trigger", "");


  /// <summary>
  /// Always return activated if it is achieved.
  /// Let all events run without returning early in case the player clears
  /// _needsReset by chance in the same frame they trigger Activated (highly
  /// unlikely this would ever happen, but w/e). 
  /// </summary>
  public override InputActionPhaseEnum Evaluate(ReadOnlySpan<InputPayload> payloadsThisFrame, double delta) {
    _elapsed += delta;

    var result = InputActionPhaseEnum.None;
    foreach (var sample in payloadsThisFrame) {
      var phase = EvaluateSample(sample);
      if (phase != InputActionPhaseEnum.Pending) _elapsed = 0;
      if (result != InputActionPhaseEnum.Activated) result = phase;
    }

    // edge case. If we early-return from all EvaluateSamples (say there's no events, or the only provided events
    // were null for some reason) we still need to check if we've timed out of our trigger window.
    if (_isPending && _elapsed > TimeWindow && result != InputActionPhaseEnum.Activated) {
      _isPending = false;
      // _needsReset stays unchanged — no sample to confirm we're below StartPoint
      result = InputActionPhaseEnum.None;
    }

    return result;
  }

  //todo: bug here. magnitude should always be positive. But question: what to do about null cases?
  protected override InputActionPhaseEnum EvaluateSample(InputPayload payload) {
    var magnitude = RestrictAxis switch {
      FlickAxis.Any => payload.Length(),
      FlickAxis.X => payload.X,
      FlickAxis.Y => payload.Y,
      FlickAxis.Z => payload.Z,
      _ => payload.Length(),
    };

    // This event doesn't touch our axis — ignore it entirely
    if (!magnitude.HasValue)
      return _isPending
        ? InputActionPhaseEnum.Pending
        : InputActionPhaseEnum.None;

    var mag = Math.Abs(magnitude.Value);

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