using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class InputManager : Node {
  private const string SavePath = "user://last_input_profile.tres";

  /*
   * These two lookups are derived from an InputProfile, recreated every time
   * a different InputProfile is used to map hardware events to bindings and
   * actions to bindings. These two lookups link us across the full pipeline:
   * hardware events (player pressed W) -> binding (collects runs its modifiers) ->
   * trigger (evaluates )
   * of
   *
   */
  /// <summary>
  /// One of two bridges for our entire pipeline. This connects the current profile's
  /// InputBindings to incoming player input events (Godot-engine level)
  /// </summary>
  private Dictionary<InputEventLookupKey, List<InputBinding>> _eventLookup;

  /// <summary>
  /// One of the two bridges for our entire pipeline. This connects the current profile's
  /// InputBindings to the final resulting InputAction (the action's trigger logic and
  /// resulting frame value). InputActions are the api for the rest of the game's code.
  /// </summary>
  private Dictionary<StringName, InputBinding> _actionLookup;

  private InputMode _currentInputMode;


  private void ApplyProfile(InputProfile newProfile) {
    var (eventLookup, actionLookup) = newProfile.GetManagerLookups();
    _eventLookup = eventLookup;
    _actionLookup = actionLookup;
  }

  private void ApplyInputMode(InputMode newMode) {
    _eventLookup.Clear();

    foreach (var actionName in newMode.ActionNames) {
      if (!InputActions.All.TryGetValue(actionName, out var inputAction))
        continue;

      // _actionLookup = 
    }

    GD.Print("For each ActionName in the mode, scan the game's Actions");
    GD.Print("For each action, trace it to its assigned binding in the given profile");
    GD.Print("build a new lookup entry");
  }

  public override void _Input(InputEvent e) {
    var key = InputEventLookupKey.From(e);
  }
}

/*
 * switch (binding)
   {
       case InputBinding simple: /* handle single * / break;
       case CompositeInputBinding composite: /* handle composite * / break;
   }
 */