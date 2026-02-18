using Godot;
using Godot.Collections;

public partial class InputMode : Resource {
  [Export] public string ModeName { get; private set; }
  [Export] public Array<StringName> ActionNames { get; private set; }
}