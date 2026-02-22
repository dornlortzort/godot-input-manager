using System;
using Godot;

public abstract partial class InputBinding : Resource {
  [Export] public InputActionName ActionName { get; protected set; }


  [Obsolete("Use parameterized constructor")]
  protected InputBinding() {
  }

  public InputBinding(InputActionName actionName) {
    ActionName = actionName;
  }
}