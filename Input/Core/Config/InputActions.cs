using Godot;
using System.Collections.Generic;

public static partial class InputActions {
  public static IReadOnlyDictionary<StringName, InputAction> All { get; internal set; }
    = new Dictionary<StringName, InputAction>();
}