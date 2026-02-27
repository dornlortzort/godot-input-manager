using Godot;

[GlobalClass]
public partial class InputManager : Node {
  [Export] public InputRegistry Registry { get; private set; }

  private CompiledInputProfile _currentProfile;
  private CompiledInputMode _currentMode;

  public override void _Ready() {
    var lastUsedProfile = InputProfileLoader.LoadLastUsedProfile(this);
    ApplyProfile(lastUsedProfile);
    if (Registry.Modes.Count > 0) {
      ApplyInputMode(Registry.Modes[0]);
    }
  }

  private void ApplyProfile(InputProfile newProfile) {
    _currentProfile = new CompiledInputProfile(newProfile);
  }

  private void ApplyInputMode(InputMode newMode) {
    _currentMode = new CompiledInputMode(newMode);
  }

  private bool IsActionInMode(InputActionName ActionName) =>
    _currentMode.Contains(ActionName) || _currentMode.Count == 0;

  public override void _Input(InputEvent e) {
    var key = InputEventLookupKey.From(e);
    // event lookups return a list of bindings, since the same event can be bound by multiple actions.
    // example: W to move fwd, double-tap W to sprint, both bind to W
    foreach (var inputBinding in _currentProfile.EventLookup[key]) {
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