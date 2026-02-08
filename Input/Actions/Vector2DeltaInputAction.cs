using System;
using Godot;

/// <summary>
///   Delta inputs are for things like mouse movement, scroll wheels, etc., where
///   "change from last frame" is measured rather than "value acquired this frame".
///   Since hardware can trigger input updates many times between frames, we accumulate
///   those deltas (in our abstract ReceiveTypedValue override). That value can be
///   read throughout the frame (in Process) before getting reset in the manager.
/// </summary>
[GlobalClass]
public partial class Vector2DeltaInputAction : InputAction<Vector2>, IDeltaInput<Vector2> {
  public override Type ValueType => typeof(Vector2);
  public override Vector2 Value { get; protected set; }

  public float Magnitude => Value.Length();

  public void ResetAccumulator() {
    Value = Vector2.Zero;
  }

  /// <summary>
  ///   Accumulates the value rather than acting as a direct setter
  /// </summary>
  public override void ReceiveTypedValue(Vector2 value) {
    Value += value;
  }
}