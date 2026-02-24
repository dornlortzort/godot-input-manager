using System;
using System.Linq;
using Godot;
using Godot.Collections;

/// <summary>
/// A simple wrapper around a string that has a custom _ValidateProperty for selecting from
/// the list of available ActionNames generated for the game from the Registry. 
/// </summary>
[Tool]
[GlobalClass]
public partial class InputActionName : Resource {
  [Export] public StringName Name { get; private set; }

  [Obsolete("Use InputActionName(StringName) instead")]
  public InputActionName() {
  }

  public InputActionName(StringName name) => Name = name;
  public InputActionName(string name) => Name = name;

  public override void _ValidateProperty(Dictionary property) {
    ResourceName = Name;
    if (property["name"].AsStringName() != PropertyName.Name) return;
    property["hint"] = (int)PropertyHint.Enum;

    if (InputActions.All == null || !InputActions.All.Keys.Any()) {
      property["hint_string"] = "No actions defined. Make an InputRegistry resource first.";
      return;
    }

    property["hint_string"] = string.Join(",", InputActions.All.Keys);
  }

  // Converts a StringName to InputActionName, e.g. InputActionName x = someStringName;
  public static implicit operator InputActionName(StringName name) => new(name);

  // Converts a string to InputActionName, e.g. InputActionName x = "jump";
  public static implicit operator InputActionName(string name) => new(name);

  // Converts an InputActionName to StringName, e.g. StringName s = someActionName;
  public static implicit operator StringName(InputActionName action) => action?.Name;

  // Converts an InputActionName to string, e.g. string s = someActionName;
  public static implicit operator string(InputActionName action) => action?.Name;

  public override string ToString() => Name;

  public override bool Equals(object obj) => obj switch {
    InputActionName other => Name == other.Name,
    StringName sn => Name == sn,
    string s => Name == s,
    _ => false
  };

  // ReSharper disable once NonReadonlyMemberInGetHashCode
  public override int GetHashCode() => Name?.GetHashCode() ?? 0;

  public static bool operator ==(InputActionName a, InputActionName b)
    => a?.Name == b?.Name;

  public static bool operator !=(InputActionName a, InputActionName b)
    => !(a == b);
}