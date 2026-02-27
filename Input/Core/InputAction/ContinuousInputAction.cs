using System;
using Godot;

public abstract partial class ContinuousInputAction<TValue> : InputAction where TValue : struct {
  public abstract TValue Value { get; }

  protected override void ConsumeSamples(ReadOnlySpan<InputSample> samplesThisFrame) {
    if (samplesThisFrame.Length == 0) {
      _currentValue = Vector3.Zero;
      return;
    }

    _currentValue = samplesThisFrame[^1].Value;
  }
}