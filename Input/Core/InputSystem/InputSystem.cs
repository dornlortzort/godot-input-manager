using Godot;

[GlobalClass]
public partial class InputSystem : Node {
  [Export] public InputRegistry Registry { get; private set; }

  private CompiledInputProfile _currentProfile;
  private CompiledInputMode _currentMode;

  public override void _Ready() {
    var lastUsedProfile = InputProfileLoader.LoadLastUsedProfile(this);
    GD.Print("InputSystem _Ready() -- lastUsedProfile load got: ", lastUsedProfile.ResourcePath);
    ApplyProfile(lastUsedProfile);
    if (Registry.Modes.Count > 0 && Registry.Modes[0].ActionNames.Count > 0) {
      GD.Print("InputSystem _Ready() -- setting mode: ", Registry.Modes[0].ModeName);
      ApplyInputMode(Registry.Modes[0]);
    } else {
      GD.Print("InputSystem _Ready() -- creating and setting default input mode (includes all input actions)");
      ApplyInputMode(InputMode.CreateDefaultInputMode());
    }
  }

  private void ApplyProfile(InputProfile newProfile) {
    _currentProfile = new CompiledInputProfile(newProfile);
    foreach (var inputEventLookupKey in _currentProfile.EventLookup.Keys) {
      GD.Print($"Applied key {inputEventLookupKey} ");
    }
  }

  private void ApplyInputMode(InputMode newMode) {
    _currentMode = new CompiledInputMode(newMode);
  }

  private bool IsActionInMode(InputActionName ActionName) =>
    _currentMode.Has(ActionName) || _currentMode.Count == 0;

  public override void _Input(InputEvent e) {
    var key = InputEventLookupKey.From(e);
    GD.Print($"InputSystem _Input() got input with key {key}");
    // event lookups return a list of bindings, since the same event can be bound by multiple actions.
    // example: W to move fwd, double-tap W to sprint, both bind to W
    if (!_currentProfile.EventLookup.TryGetValue(key, out var bindings)) return;

    foreach (var inputBinding in bindings) {
      //skip actions not in the current mode
      if (!IsActionInMode(inputBinding.ActionName)) continue;
      inputBinding.CaptureEvent(e);
    }
  }

  //todo: figure out how chord triggers complicate this process. 
  // add a deferral for chord trigger evaluations whenever the chord's
  // constituents haven't run?
  public override void _Process(double delta) {
    // pass off all bindings' values to their respective action
    foreach (var actionName in _currentMode.ActionNames) {
      var inputBinding = _currentProfile.ActionLookup[actionName];
      var action = InputActions.All[actionName];
      inputBinding.DrainTo(action, delta);
    }
  }
}