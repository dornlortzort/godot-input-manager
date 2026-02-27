using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Bridges (a) an InputProfile's InputBindings to the game's InputActions and
/// (b) an InputProfile's InputBindings to Godot's InputEvents (i.e. events coming in from
/// hardware).
///
/// Effectively this is how our InputManager equips its profile and hooks into game
/// updates.
/// </summary>
public class CompiledInputProfile {
  /// <summary>
  /// Game's InputActions => Profile's InputBindings
  /// </summary>
  public Dictionary<InputActionName, InputBinding> ActionLookup { get; }

  /// <summary>
  /// Godot-engine InputEvents => Profile's InputBindings
  /// </summary>
  public Dictionary<InputEventLookupKey, List<SimpleInputBinding>> EventLookup { get; }


  public CompiledInputProfile(InputProfile profile) {
    if (!profile.IsValid(out string error)) {
      GD.PushError($"{error} -- Returning default values instead. ");
      throw new InvalidOperationException(
        "InputProfile is invalid. To debug, view it in the editor and click \"Validate Profile\"");
    }

    EventLookup = new Dictionary<InputEventLookupKey, List<SimpleInputBinding>>();
    ActionLookup = new Dictionary<InputActionName, InputBinding>();

    foreach (var binding in profile.AllBindings) {
      if (binding == null)
        throw new InvalidOperationException("InputProfile contains a null binding.");

      switch (binding) {
        case SimpleInputBinding simple:
          AddToEventLookup(EventLookup, simple);
          break;
        case CompositeInputBinding composite:
          foreach (var child in composite.Bindings) {
            // this line is sneaky important: links a composite to its child
            child.Parent = composite;
            AddToEventLookup(EventLookup, child);
          }

          break;
      }

      ActionLookup[binding.ActionName] = binding;
    }
  }

  /// <summary>
  /// Helper function
  /// </summary>
  /// <param name="lookup">the lookup mid-construction</param>
  /// <param name="binding">binding to add</param>
  private static void
    AddToEventLookup(Dictionary<InputEventLookupKey, List<SimpleInputBinding>> lookup, SimpleInputBinding binding) {
    var key = InputEventLookupKey.From(binding.SourceEvent);
    if (!lookup.TryGetValue(key, out var list)) {
      list = [];
      lookup[key] = list;
    }

    list.Add(binding);
  }
}