using Godot;
using Godot.Collections;

public partial class CompositeInputBinding : Resource {
  [Export] public Array<InputBinding> Bindings { get; private set; }
  [Export] public Array<InputModifier> Modifiers { get; private set; }
}