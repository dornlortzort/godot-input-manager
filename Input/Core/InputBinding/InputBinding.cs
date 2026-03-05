using Godot;


public enum DeviceTypeEnum {
  None,
  Unsupported,
  KeyboardMouse,
  Joypad,
}

[Tool]
[GlobalClass]
public abstract partial class InputBinding : Resource, ICustomNamedResource {
  public abstract InputActionName ActionName { get; }

  public abstract string GetResourceName();

  public abstract void DrainTo(InputAction action, double delta);

  /*
   *
   * Tooling
   *
   */
  public static string GetBindingSourceName(IEventCapturableBinding binding) => binding is null
    ? "null"
    : binding.SourceEvent switch {
      InputEventKey key => key.PhysicalKeycode != Key.None
        ? key.PhysicalKeycode.ToString()
        : key.Keycode.ToString(),
      InputEventMouseButton mb => $"{mb.ButtonIndex} MB",
      InputEventJoypadButton jb => jb.ButtonIndex.ToString(),
      InputEventJoypadMotion jm => $"Axis{jm.Axis} {jm.AxisValue}",
      _ => binding.SourceEvent?.GetType().Name ?? "Unset"
    };

  public static DeviceTypeEnum GetBindingDeviceType(IEventCapturableBinding binding) => binding is null
    ? DeviceTypeEnum.None
    : binding.SourceEvent switch {
      InputEventKey or InputEventMouseButton or InputEventMouseMotion => DeviceTypeEnum.KeyboardMouse,
      InputEventJoypadButton or InputEventJoypadMotion => DeviceTypeEnum.Joypad,
      _ => binding.SourceEvent is null ? DeviceTypeEnum.None : DeviceTypeEnum.Unsupported
    };
}