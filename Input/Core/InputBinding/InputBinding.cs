using Godot;


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
  public static string GetInputSourceName(InputEvent source) => source switch {
    InputEventKey key => key.PhysicalKeycode != Key.None
      ? key.PhysicalKeycode.ToString()
      : key.Keycode.ToString(),
    InputEventMouseButton mb => $"{mb.ButtonIndex} MB",
    InputEventJoypadButton jb => jb.ButtonIndex.ToString(),
    InputEventJoypadMotion jm => $"Axis{jm.Axis} {jm.AxisValue}",
    _ => source?.GetType().Name ?? "Unset"
  };
}