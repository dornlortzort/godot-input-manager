using System;
using Godot;

[Tool]
[GlobalClass]
public partial class DownTrigger : InputTrigger {
  [Export] public float Threshold { get; set; } = 0.3f;


  private bool _isActive;

  public override string AsCodeDeclarationString() => $"new {nameof(DownTrigger)}() {{ Threshold = {Threshold}f }}";
  public override string GetResourceName() => nameof(DownTrigger).Replace("Trigger", "");

  /// <summary>
  /// Return the "high water mark" this frame.
  /// </summary>
  public override InputActionPhaseEnum Evaluate(ReadOnlySpan<InputSample> samplesThisFrame, double delta) {
    var result = InputActionPhaseEnum.None;
    foreach (var sample in samplesThisFrame) {
      var phase = EvaluateSample(sample);
      if (phase > result) result = phase;
    }

    return result;
  }

  protected override InputActionPhaseEnum EvaluateSample(InputSample sample) {
    var magnitude = sample.Value.Length();
    var isAbove = magnitude >= Threshold;
    return isAbove ? InputActionPhaseEnum.Activated : InputActionPhaseEnum.None;
  }

  public override InputTrigger Clone() {
    var clone = (DownTrigger)Duplicate();
    clone.Reset();
    return clone;
  }

  public override void Reset() {
    _isActive = false;
  }
}