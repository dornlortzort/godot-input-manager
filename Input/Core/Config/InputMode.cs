using System.Collections.Generic;
using Godot;
using Godot.Collections;

// todo: build out a tool for auto-adding any actions that contain the
//  currently typed keyword
[Tool]
[GlobalClass]
public partial class InputMode : Resource {
  [Export] public string ModeName { get; private set; }
  [Export] public Array<InputActionName> ActionNames { get; private set; }
  [Export] public string AddActionsMatching { get; private set; }

  [ExportToolButton("Add", Icon = "Add")]
  private Callable AddActionsButton => Callable.From(AddActionsThatMatchKeyword);

  public bool IsValid(out string error) {
    var invalid = new List<string>();
    var valid = 0;

    foreach (var actionName in ActionNames) {
      if (InputActions.All.ContainsKey(actionName))
        valid++;
      else
        invalid.Add(actionName.ToString());
    }

    if (invalid.Count > 0) {
      error =
        $"{invalid.Count} of {ActionNames.Count} actions are invalid: [{string.Join(", ", invalid)}].";
      return false;
    }

    error = null;
    return true;
  }

  private void AddActionsThatMatchKeyword() {
    if (string.IsNullOrEmpty(AddActionsMatching)) {
      GD.PushWarning("No keyword specified.");
      return;
    }

    var count = 0;
    foreach (var actionName in InputActions.All.Keys) {
      if (!actionName.ToString().Contains(AddActionsMatching)) continue;
      if (ActionNames.Contains(actionName)) continue;
      ActionNames.Add(actionName);
      count++;
    }

    GD.Print($"Added {count} action(s) that matched \"{AddActionsMatching}\".");
    AddActionsMatching = "";
  }
}