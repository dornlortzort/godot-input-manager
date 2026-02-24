using System;
using Godot;

[GlobalClass]
public partial class PressTrigger : InputTrigger {
  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float Threshold { get; set; } = 0.5f;

  public override string AsCodeDeclarationString() {
    return $"new {nameof(PressTrigger)}() {{ Threshold = {Threshold}f }}";
  }

  private bool _wasAbove;

  /// <summary>
  /// Return the "high water mark" this frame.
  /// </summary>
  public override InputActionPhaseEnum Evaluate(ReadOnlySpan<InputSample> samplesThisFrame, float delta) {
    var result = InputActionPhaseEnum.None;
    foreach (var sample in samplesThisFrame) {
      var phase = EvaluateSample(sample);
      if (result < phase) result = phase;
    }

    return result;
  }

  protected override InputActionPhaseEnum EvaluateSample(InputSample sample) {
    var magnitude = sample.Value.Length();

    var isAbove = magnitude >= Threshold;
    InputActionPhaseEnum result;

    if (isAbove && !_wasAbove)
      result = InputActionPhaseEnum.Activated;
    else
      result = InputActionPhaseEnum.None;

    _wasAbove = isAbove;
    return result;
  }

  public override InputTrigger Clone() {
    var clone = (PressTrigger)Duplicate();
    clone.Reset();
    return clone;
  }

  public override void Reset() {
    _wasAbove = false;
  }
}