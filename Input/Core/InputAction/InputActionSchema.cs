using System;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class InputActionSchema : Resource, ICustomNamedResource {
  [Export] private InputActionValueEnum _value = InputActionValueEnum.Bool;
  [Export] private StringName _actionName = "ActionName";
  [Export] private string _displayName = "DisplayName";

  [Export] private bool _isRemappable = true;
  [Export] private float _bufferSeconds;
  [Export] private bool _isDeltaInput;
  [Export] private InputTrigger _trigger = new DownTrigger();


  /// <summary>
  /// Auto-updates the displayed name in the editor for easier readability
  /// </summary>
  public override void _ValidateProperty(Dictionary property) {
    ResourceName = GetResourceName();
  }

  public string GetResourceName() {
    var buf = _bufferSeconds > 0 ? $"{_bufferSeconds}s" : "✗";
    var remap = _isRemappable ? "✓" : "✗";
    var d = _isDeltaInput ? "Δ" : "";

    return $"{d}{_value} '{_actionName}' [{_trigger.GetResourceName()}], remap:{remap}, buf:{buf}";
  }

  public string AsCodeDeclarationString() {
    var className = (IsDeltaInput: _isDeltaInput, Value: _value) switch {
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

    return $"    public static readonly {className} {_actionName} = new() {{ "
           + $"ActionName = \"{_actionName}\", "
           + $"DisplayName = \"{_displayName}\", "
           + $"ValueType = {nameof(InputActionValueEnum)}.{_value}, "
           + $"IsRemappable = {_isRemappable.ToString().ToLower()}, "
           + $"BufferSeconds = {_bufferSeconds}f, "
           + $"Trigger = {_trigger.AsCodeDeclarationString()} "
           + $"}};";
  }

  public string AsDictionaryEntryString() {
    return $"        {{ \"{_actionName}\", {_actionName} }},";
  }

  private int GetHash() => GD.Hash(AsCodeDeclarationString());
}