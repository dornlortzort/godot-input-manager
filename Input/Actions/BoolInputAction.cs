using System;

/// <summary>
///   Can't make generic classes as Godot [GlobalClass] types, but you can make their
///   typed children generic
/// </summary>
public partial class BoolInputAction : InputAction<bool>, IReadableInput<bool> {
  public bool Value { get; private set; }
  // todo: add a default trigger (optional)
  //    type consideration: should this be a Resource as well?

  public override Type ValueType => typeof(bool);

  public override void ReceiveValue(bool value, float delta) {
    throw new NotImplementedException();
  }
}