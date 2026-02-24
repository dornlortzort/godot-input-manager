using System;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class InputActionSchema : Resource {
  [Export] private InputActionValueEnum _value = InputActionValueEnum.Bool;
  [Export] private StringName _actionName = "ActionName";
  [Export] private string _displayName = "DisplayName";

  [Export] private bool _isRemappable = true;
  [Export] private float _bufferSeconds;
  [Export] private bool _isDeltaInput;

  // todo: replace with `[Export] private InputTrigger _trigger` each new release to see if they've fixed it.
  [Export] private Resource _triggerResource = new DownTrigger();

  private InputTrigger _trigger;

  public InputTrigger Trigger {
    get => _trigger ??= _triggerResource as InputTrigger ?? new DownTrigger();
    set {
      _triggerResource = value;
      _trigger = value;
    }
  }

  /// <summary>
  /// Auto-updates the displayed name in the editor for easier readability
  /// </summary>
  public override void _ValidateProperty(Dictionary property) {
    // todo: this also simplifies if the above fix is made to polymorphic exports. 
    var triggerScript = _triggerResource?.GetScript() ?? new Variant();
    var triggerName = "DownTrigger";
    if (triggerScript.VariantType != Variant.Type.Nil) {
      triggerName = triggerScript.As<Script>().GetGlobalName();
    }

    ResourceName =
      $"({(_isDeltaInput ? "Δ" : "")}{_value}) [{triggerName}] '{_actionName}' | remap: {(_isRemappable ? "y" : "n")} | buf: {(_bufferSeconds > 0 ? $"{_bufferSeconds}s" : "none")}";
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
           + $"Trigger = {Trigger.AsCodeDeclarationString()} "
           + $"}};";
  }

  public string AsDictionaryEntryString() {
    return $"        {{ \"{_actionName}\", {_actionName} }},";
  }

  public int GetHash() => GD.Hash(AsCodeDeclarationString());
}