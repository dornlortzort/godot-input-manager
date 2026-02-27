using System;
using Godot;

public abstract partial class InputBinding : Resource, ICustomNamedResource {
  [Export] public InputActionName ActionName { get; protected set; }

  [Obsolete("Use parameterized constructor")]
  protected InputBinding() {
  }

  protected InputBinding(InputActionName actionName) {
    ActionName = actionName;
  }

  public abstract string GetResourceName();

  public abstract void DrainTo(InputAction action, double delta);
}