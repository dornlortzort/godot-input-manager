using System;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class InputActionSchema : Resource {
  [Export] private InputActionValueEnum Value = InputActionValueEnum.Bool;
  [Export] private StringName ActionName = "ActionName";
  [Export] private string DisplayName = "DisplayName";

  [Export] private bool IsRemappable = true;
  [Export] private float BufferSeconds = 0f;
  [Export] private bool IsDeltaInput = false;

  [Export] private InputTrigger _trigger;

  /// <summary>
  /// Auto-updates the displayed name in the editor for easier readability
  /// </summary>
  public override void _ValidateProperty(Dictionary property) {
    ResourceName =
      $"{(IsDeltaInput ? "Δ" : "")}{Value} {ActionName} '{DisplayName}' | remap: {(IsRemappable ? "y" : "n")} | buf: {(BufferSeconds > 0 ? $"{BufferSeconds}s" : "none")}";
  }

  public string AsCodeDeclarationString() {
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
           + $"ValueType = {nameof(InputActionValueEnum)}.{Value}, "
           + $"IsRemappable = {IsRemappable.ToString().ToLower()}, "
           + $"BufferSeconds = {BufferSeconds}f, "
           + $"Trigger = {_trigger.AsCodeDeclarationString()} "
           + $"}};";
  }

  public string AsDictionaryEntryString() {
    return $"        {{ \"{ActionName}\", {ActionName} }},";
  }

  public int GetHash() => GD.Hash(AsCodeDeclarationString());
}