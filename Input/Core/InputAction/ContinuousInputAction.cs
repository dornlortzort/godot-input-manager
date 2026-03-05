using System;
using Godot;

public abstract partial class ContinuousInputAction<TValue> : InputAction where TValue : struct {
  public abstract TValue Value { get; }

  protected override void ConsumePayloads(ReadOnlySpan<InputPayload> payloadsThisFrame) {
    if (payloadsThisFrame.Length == 0) return;

    var latest = payloadsThisFrame[^1];
    _currentValue = new Vector3(latest.X ?? _currentValue.X, latest.Y ?? _currentValue.Y, latest.Z ?? _currentValue.Z);
  }
}