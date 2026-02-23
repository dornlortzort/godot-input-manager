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
  [Export] public string Keyword { get; private set; }

  [ExportToolButton("Add Actions Matching Keyword", Icon = "Add")]
  private Callable AddActionsButton => Callable.From(AddActionsThatMatchKeyword);

  [ExportToolButton("Remove Actions Matching Keyword", Icon = "Remove")]
  private Callable RemoveActionsButton => Callable.From(RemoveActionsThatMatchKeyword);

  public bool IsValid(out string error) {
    var invalid = new List<string>();
    foreach (var actionName in ActionNames) {
      if (!InputActions.All.ContainsKey(actionName))
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
    if (string.IsNullOrEmpty(Keyword)) {
      GD.PushWarning("No keyword specified.");
      return;
    }

    var count = 0;
    foreach (var actionName in InputActions.All.Keys) {
      //no match
      if (!actionName.ToString().Contains(Keyword)) continue;
      //duplicate
      if (ActionNames.Contains(actionName)) continue;
      ActionNames.Add(actionName);
      count++;
    }

    GD.Print($"Added {count} action(s) that matched \"{Keyword}\".");
    Keyword = "";
  }

  private void RemoveActionsThatMatchKeyword() {
    if (string.IsNullOrEmpty(Keyword)) {
      GD.PushWarning("No keyword specified.");
      return;
    }

    var count = 0;
    for (var i = ActionNames.Count - 1; i >= 0; i--) {
      if (!ActionNames[i].ToString().Contains(Keyword)) continue;
      ActionNames.RemoveAt(i);
      count++;
    }

    GD.Print($"Removed {count} action(s) matching \"{Keyword}\".");
    Keyword = "";
  }
}