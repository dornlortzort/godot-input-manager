using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class InputManager : Node {
  [Export] public InputRegistry Registry { get; private set; }

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


  /*
   * todo: review this load profile and the load/save logic below.
   *  improve and consolidate logic for InputModes (answer: is _eventLookup for
   *  the mode or for the whole profile?)
   *  - argument could be made that input mode should have a HashSet it exposes. (<-- yes  this)
   */
  private InputProfile LoadProfile() {
    if (ResourceLoader.Exists(SavePath)) {
      var loaded = ResourceLoader.Load<InputProfile>(SavePath);
      if (loaded != null) return loaded;
      GD.PushWarning($"Corrupted profile at {SavePath}, falling back to default.");
    }

    if (Registry != null) return Registry.DefaultProfile;

    throw new System.InvalidOperationException("No valid input Registry or profile available.");
  }

  private void SaveProfilePath(string profilePath) {
    using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
    file.StoreString(profilePath);
  }

  private string LoadProfilePath() {
    if (!FileAccess.FileExists(SavePath)) return null;
    using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
    return file.GetAsText().StripEdges();
  }

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