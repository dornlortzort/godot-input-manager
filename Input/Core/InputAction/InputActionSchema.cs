using System;
using Godot;

[GlobalClass]
public partial class InputActionSchema : Resource {
  [Export] public InputActionValueEnum ValueEnum { get; private set; }
  [Export] public StringName ActionName { get; private set; }
  [Export] public string DisplayName { get; private set; }
  [Export] public bool IsRemappable { get; private set; }
  [Export] public float BufferSeconds { get; private set; }
  [Export] public bool IsDeltaInput { get; private set; }

  public string AsStringCodeDeclaration() {
    var className = (IsDeltaInput, ValueType: ValueEnum) switch {
      (false, InputActionValueEnum.Bool) => "ContinuousBoolAction",
      (false, InputActionValueEnum.Axis1D) => "ContinuousAxis1DAction",
      (false, InputActionValueEnum.Vector2) => "ContinuousVector2Action",
      (false, InputActionValueEnum.Vector3) => "ContinuousVector3Action",
      (true, InputActionValueEnum.Bool) => "DeltaBoolAction",
      (true, InputActionValueEnum.Axis1D) => "DeltaAxis1DAction",
      (true, InputActionValueEnum.Vector2) => "DeltaVector2Action",
      (true, InputActionValueEnum.Vector3) => "DeltaVector3Action",
      _ => throw new ArgumentOutOfRangeException()
    };

    return $"    public static readonly {className} {ActionName} = new() {{ "
           + $"ActionName = \"{ActionName}\", "
           + $"DisplayName = \"{DisplayName}\", "
           + $"IsRemappable = {IsRemappable.ToString().ToLower()}, "
           + $"BufferSeconds = {BufferSeconds}f "
           + $"}};";
  }

  public int GetHash() => GD.Hash(AsStringCodeDeclaration());
}