using Godot;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public class CompiledInputMode(InputMode mode) {
  public StringName ModeName { get; } = mode.ModeName;
  public ImmutableArray<InputActionName> ActionNames { get; } = [..mode.ActionNames];
  public HashSet<InputActionName> ActionNameSet { get; } = mode.ActionNames.ToHashSet();

  public int Count => ActionNames.Length;

  public bool Has(StringName actionName) =>
    ActionNameSet.Contains(actionName);
}