using Godot;
using System;

public abstract partial class DeltaInputAction<TValue> : BaseInputAction where TValue : struct {
  /// <summary>
  /// FrameDelta represents "change from last frame" rather than "value acquired this frame".
  /// Since hardware can trigger input updates many times between frames, we accumulate
  /// those deltas (in an abstract ReceiveValue override). That value can be
  /// read throughout the frame (in Process) before getting reset in the manager.
  /// </summary>
  public abstract TValue FrameDelta { get; }

  /// <summary>
  /// Accumulates the input pipeline data's value, as opposed to setting it directly. 
  /// </summary>
  /// <param name="input"></param>
  public override void ReceiveValue(InputPipelineData input) {
    _currentValue += input.Value;
  }

  public void ResetAccumulator() {
    _currentValue = Vector3.Zero;
  }
}