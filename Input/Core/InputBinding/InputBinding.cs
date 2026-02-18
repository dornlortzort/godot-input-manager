using Godot;
using Godot.Collections;

public partial class InputBinding : Resource {
  [Export] public InputEvent Source { get; private set; }
  [Export] public Array<InputModifier> modifiers { get; private set; }

  [Export] private InputTrigger _trigger;
  private DownTrigger _defaultTrigger = new();

  public InputTrigger Trigger {
    get => _trigger ?? _defaultTrigger;
    set => _trigger = value;
  }
}