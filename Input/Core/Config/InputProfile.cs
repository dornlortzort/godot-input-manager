using System;
using System.Collections.Generic;
using System.Text;
using Godot;

/// <summary>
/// InputProfile offers an editor interface for binding all of the game's
/// input actions 
/// </summary>
[Tool]
[GlobalClass]
public partial class InputProfile : Resource {
  [Export] private Godot.Collections.Array<InputBinding> _allBindings;


  /*
   *
   * API: Primarily used by the InputManager autoload
   *
   */
  /// <summary>
  /// Bridges (a) an InputProfile's InputBindings to the game's InputActions and (b)
  /// an InputProfile's InputBindings to Godot's InputEvents (i.e. events coming in from
  /// hardware).
  ///
  /// Effectively this is how our InputManager equips its profile and hooks into game
  /// updates.
  /// </summary>
  public (
    Dictionary<InputEventLookupKey, List<InputBinding>> EventLookup, Dictionary<StringName, InputBinding> ActionLookup)
    GetManagerLookups() {
    if (!IsValid(out string error)) {
      GD.PushError($"{error} -- Returning default values instead. ");
      throw new InvalidOperationException(
        "InputProfile is invalid. To debug, view it in the editor and click \"Validate Profile\"");
    }

    var eventLookup = new Dictionary<InputEventLookupKey, List<InputBinding>>();
    var actionLookup = new Dictionary<StringName, InputBinding>();

    foreach (var binding in _allBindings) {
      if (binding == null)
        throw new InvalidOperationException("InputProfile contains a null binding.");

      switch (binding) {
        case SimpleInputBinding simple:
          AddToEventLookup(eventLookup, simple);
          break;
        case CompositeInputBinding composite:
          foreach (var child in composite.Bindings)
            AddToEventLookup(eventLookup, child);
          break;
      }

      actionLookup[binding.ActionName] = binding;
    }

    return (eventLookup, actionLookup);
  }


  /// <summary>
  /// Helper function
  /// </summary>
  /// <param name="lookup">the lookup mid-construction</param>
  /// <param name="binding">binding to add</param>
  private static void
    AddToEventLookup(Dictionary<InputEventLookupKey, List<InputBinding>> lookup, SimpleInputBinding binding) {
    var key = InputEventLookupKey.From(binding.SourceEvent);
    if (!lookup.TryGetValue(key, out var list)) {
      list = [];
      lookup[key] = list;
    }

    list.Add(binding);
  }

  /*
   *
   * Validation: does my input profile have a binding for all of the game's actions?
   *
   *
   */

  [ExportToolButton("Validate Profile")]
  private Callable ValidateProfileButton => Callable.From(ValidateProfileAndPrintResult);

  private void ValidateProfileAndPrintResult() {
    if (!IsValid(out string error)) {
      GD.PushError($"Profile Invalid. Reason: {error}");
    }

    GD.Print("Profile is valid!");
  }

  public bool IsValid(out string error, string tabs = "") {
    var bound = GetBoundActionNames();
    var missing = FindMissingActions(bound);

    if (missing.Count == 0) {
      error = null;
      return true;
    }

    var sb = new StringBuilder($"{tabs}Invalid profile. Missing bindings:");
    foreach (var action in missing)
      sb.AppendLine($"{tabs}  - {action.ActionName} ({action.ValueType})");

    error = sb.ToString();
    return false;
  }

  /*
   *
   * Automation:
   *
   */

  [ExportToolButton("Auto-Populate Profile Bindings")]
  private Callable AutoPopulateButton => Callable.From(AutoPopulateMissingBindings);

  private void AutoPopulateMissingBindings() {
    _allBindings ??= [];
    var bound = GetBoundActionNames();
    var missing = FindMissingActions(bound);

    foreach (var action in missing)
      _allBindings.Add(CreateDefaultBinding(action));
  }

  /*
   *
   * Helpers
   *
   */
  private HashSet<StringName> GetBoundActionNames() {
    var bound = new HashSet<StringName>();
    if (_allBindings == null) return bound;

    foreach (var binding in _allBindings) {
      if (binding?.ActionName is not null) {
        bound.Add(binding.ActionName);
        continue;
      }

      GD.PushWarning("A binding was found without an ActionName. You better be fixing this.");
    }

    return bound;
  }

  private static List<InputAction> FindMissingActions(HashSet<StringName> bound) {
    var missing = new List<InputAction>();
    foreach (var action in InputActions.All.Values) {
      if (!bound.Contains(action.ActionName))
        missing.Add(action);
    }

    return missing;
  }

  private static InputBinding CreateDefaultBinding(InputAction action) {
    return action.ValueType switch {
      InputActionValueEnum.Bool => new SimpleInputBinding(action.ActionName,
        new InputEventKey() { Keycode = Key.Space }),
      InputActionValueEnum.Axis1D => new SimpleInputBinding(action.ActionName, new InputEventJoypadMotion()),
      InputActionValueEnum.Vector2 => GetDefault2D(action.ActionName),
      InputActionValueEnum.Vector3 => GetDefault3D(action.ActionName),
      _ => throw new ArgumentOutOfRangeException()
    };
  }

  private static CompositeInputBinding GetDefault2D(InputActionName actionName) => new(actionName, [
    new SimpleInputBinding(null, new InputEventKey() { Keycode = Key.W },
      [new SwizzleModifier(order: SwizzleModifier.SwizzleOrder.YXZ)]),

    new SimpleInputBinding(null, new InputEventKey() { Keycode = Key.A },
      [new NegateModifier()]),

    new SimpleInputBinding(null, new InputEventKey() { Keycode = Key.S },
      [new SwizzleModifier(order: SwizzleModifier.SwizzleOrder.YXZ), new NegateModifier()]),

    new SimpleInputBinding(null, new InputEventKey() { Keycode = Key.D },
      [])
  ]);

  private static CompositeInputBinding GetDefault3D(InputActionName actionName) => new(actionName, [
    new SimpleInputBinding(null, new InputEventKey() { Keycode = Key.W },
      [new SwizzleModifier(order: SwizzleModifier.SwizzleOrder.ZYX), new NegateModifier()]),

    new SimpleInputBinding(null, new InputEventKey() { Keycode = Key.A },
      [new NegateModifier()]),

    new SimpleInputBinding(null, new InputEventKey() { Keycode = Key.S },
      [new SwizzleModifier(order: SwizzleModifier.SwizzleOrder.ZYX)]),

    new SimpleInputBinding(null, new InputEventKey() { Keycode = Key.D },
      [])
  ]);
}