using System;
using Godot;

[GlobalClass]
public partial class Vector2InputAction : InputAction<Vector2> {
  public override Type ValueType => typeof(Vector2);
  public override Vector2 Value { get; protected set; }

  public float Magnitude => Value.Length();

  public override void ReceiveTypedValue(Vector2 value) {
    Value = value;
  }
}