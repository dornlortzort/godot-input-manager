using System;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class InputActionSchema : Resource {
  [Export] public InputActionValueEnum Value { get; private set; } = InputActionValueEnum.Bool;
  [Export] public StringName ActionName { get; private set; } = "ActionName";
  [Export] public string DisplayName { get; private set; } = "DisplayName";

  [Export] public bool IsRemappable { get; private set; } = true;
  [Export] public float BufferSeconds { get; private set; }
  [Export] public bool IsDeltaInput { get; private set; }

  /// <summary>
  /// Auto-updates the displayed name in the editor for easier readability
  /// </summary>
  public override void _ValidateProperty(Dictionary property) {
    ResourceName =
      $"{(IsDeltaInput ? "Δ" : "")}{Value} {ActionName} '{DisplayName}' - remap: {(IsRemappable ? "y" : "n")} | buf: {(BufferSeconds > 0 ? $"{BufferSeconds}s" : "none")}";
  }

  public string AsStringCodeDeclaration() {
    var className = (IsDeltaInput, Value) switch {
      (false, InputActionValueEnum.Bool) => "BoolInputAction",
      (false, InputActionValueEnum.Axis1D) => "FloatInputAction",
      (false, InputActionValueEnum.Vector2) => "Vector2InputAction",
      (false, InputActionValueEnum.Vector3) => "Vector3InputAction",
      (true, InputActionValueEnum.Bool) => "DeltaBoolInputAction",
      (true, InputActionValueEnum.Axis1D) => "DeltaFloatInputAction",
      (true, InputActionValueEnum.Vector2) => "DeltaVector2InputAction",
      (true, InputActionValueEnum.Vector3) => "DeltaVector3InputAction",
      _ => throw new ArgumentOutOfRangeException()
    };

    return $"    public static readonly {className} {ActionName} = new() {{ "
           + $"ActionName = \"{ActionName}\", "
           + $"DisplayName = \"{DisplayName}\", "
           + $"IsRemappable = {IsRemappable.ToString().ToLower()}, "
           + $"BufferSeconds = {BufferSeconds}f "
           + $"}};";
  }

  public string AsStringDictionaryEntry() {
    return $"        {{ \"{ActionName}\", {ActionName} }},";
  }

  public int GetHash() => GD.Hash(AsStringCodeDeclaration());
}