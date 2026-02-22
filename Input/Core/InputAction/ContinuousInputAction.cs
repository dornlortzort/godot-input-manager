using System;

public abstract partial class ContinuousInputAction<TValue> : InputAction where TValue : struct {
  public abstract TValue Value { get; }

  protected override void ConsumeSamples(ReadOnlySpan<InputSample> samplesThisFrame) {
    _currentValue = samplesThisFrame[^1].Value;
  }
}