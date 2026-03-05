using System;
using Godot;

/// <summary>
///   HoldUntilTrigger is the same as DownTrigger but you have to meet a Duration
///   requirement
/// </summary>
[Tool]
[GlobalClass]
public partial class HoldUntilTrigger : InputTrigger {
  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float Threshold { get; set; } = 0.3f;

  [Export] public float Duration { get; set; } = 0.5f;

  private InputActionPhaseEnum _stateLastFrame;
  private double _elapsed;

  public override string AsCodeDeclarationString() {
    return $"new {nameof(HoldUntilTrigger)}() {{ "
           + $"Threshold = {Threshold}f, "
           + $"Duration = {Duration}f"
           + $" }}";
  }

  public override string GetResourceName() => nameof(HoldUntilTrigger).Replace("Trigger", "");

  /// <summary>
  /// if any sample this frame evaluates to None, reset _elapsed.
  /// return the most recent (i.e. last) sample's result.
  /// </summary>
  public override InputActionPhaseEnum Evaluate(ReadOnlySpan<InputPayload> payloadsThisFrame, double delta) {
    _elapsed += delta;

    var result = InputActionPhaseEnum.None;
    foreach (var sample in payloadsThisFrame) {
      result = EvaluateSample(sample);
      if (result == InputActionPhaseEnum.None) _elapsed = 0;
    }

    return result;
  }

  protected override InputActionPhaseEnum EvaluateSample(InputPayload payload) {
    var magnitude = payload.Length();

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