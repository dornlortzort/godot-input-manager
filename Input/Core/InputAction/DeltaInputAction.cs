using Godot;
using System;

public abstract partial class DeltaInputAction<TValue> : InputAction where TValue : struct {
  /// <summary>
  /// FrameDelta represents "change from last frame" rather than "value acquired this frame".
  /// Since hardware can trigger input updates many times between frames, we accumulate
  /// those deltas (in an abstract ReceiveValue override). That value can be
  /// read throughout the frame (in Process) before getting reset in the InputSystem.
  /// </summary>
  public abstract TValue FrameDelta { get; }

  /// <summary>
  /// Accumulates the input pipeline data's value, as opposed to setting it directly. 
  /// </summary>
  protected override void ConsumePayloads(ReadOnlySpan<InputPayload> payloadsThisFrame) {
    _currentValue = Vector3.Zero;
    foreach (ref readonly var p in payloadsThisFrame)
      _currentValue += new Vector3(p.X ?? 0f, p.Y ?? 0f, p.Z ?? 0f);
  }
}