using System;
using Godot;

[GlobalClass]
public partial class BoolInputAction : InputAction<bool> {
  public override Type ValueType => typeof(bool);
  public override bool Value { get; protected set; }

  public override void ReceiveTypedValue(bool value) {
    Value = value;
  }
}