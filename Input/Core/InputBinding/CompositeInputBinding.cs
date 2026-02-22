using System;
using Godot;
using Godot.Collections;

public partial class CompositeInputBinding : InputBinding {
  [Export] public Array<SimpleInputBinding> Bindings { get; private set; }
  [Export] public Array<InputModifier> Modifiers { get; private set; }

  [Obsolete("Use parameterized constructor")]
  public CompositeInputBinding() {
  }

  public CompositeInputBinding(InputActionName actionName, Array<SimpleInputBinding> bindings,
    Array<InputModifier> modifiers = null)
    : base(actionName) {
    Bindings = bindings;
    Modifiers = modifiers ?? new();
  }
}