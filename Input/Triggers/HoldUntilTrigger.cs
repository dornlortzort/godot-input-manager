using System;
using Godot;

/// <summary>
///   HoldUntilTrigger is the same as DownTrigger but you have to meet a Duration
///   requirement
/// </summary>
[GlobalClass]
public partial class HoldUntilTrigger : InputTrigger {
  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float Threshold { get; set; } = 0.3f;

  [Export] public float Duration { get; set; } = 0.5f;

  public override string AsCodeDeclarationString() {
    return $"new {nameof(HoldUntilTrigger)}() {{ "
           + $"Threshold = {Threshold}f, "
           + $"Duration = {Duration}f"
           + $" }}";
  }

  private InputActionPhaseEnum _stateLastFrame;
  private float _elapsed;

  /// <summary>
  /// if any sample this frame evaluates to None, reset _elapsed.
  /// return the most recent (i.e. last) sample's result.
  /// </summary>
  public override InputActionPhaseEnum Evaluate(ReadOnlySpan<InputSample> samplesThisFrame, float delta) {
    _elapsed += delta;

    var result = InputActionPhaseEnum.None;
    foreach (var sample in samplesThisFrame) {
      result = EvaluateSample(sample);
      if (result == InputActionPhaseEnum.None) _elapsed = 0;
    }

    return result;
  }

  protected override InputActionPhaseEnum EvaluateSample(InputSample sample) {
    var magnitude = sample.Value.Length();

    var isAbove = magnitude >= Threshold;
    if (_stateLastFrame == InputActionPhaseEnum.None) {
      return !isAbove ? InputActionPhaseEnum.None : InputActionPhaseEnum.Pending;
    }

    if (_stateLastFrame == InputActionPhaseEnum.Pending) {
      if (!isAbove) return InputActionPhaseEnum.None;
      return _elapsed < Duration ? InputActionPhaseEnum.Pending : InputActionPhaseEnum.Activated;
    }

    // reaching here means was activated last frame.
    return isAbove ? InputActionPhaseEnum.Activated : InputActionPhaseEnum.None;
  }

  public override InputTrigger Clone() {
    var clone = (HoldUntilTrigger)Duplicate();
    clone.Reset();
    return clone;
  }

  public override void Reset() {
    _elapsed = 0.0f;
    _stateLastFrame = InputActionPhaseEnum.None;
  }
}