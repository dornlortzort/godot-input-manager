using System;
using Godot;

[GlobalClass]
public partial class FloatInputAction : InputAction<float> {
  public override Type ValueType => typeof(float);
  public override float Value { get; protected set; }

  public float Magnitude => Math.Abs(Value);

  public override void ReceiveTypedValue(float value) {
    Value = value;
  }
}