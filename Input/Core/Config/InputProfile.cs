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
  [Export] public Godot.Collections.Array<InputBinding> AllBindings { get; private set; }

  /*
   *
   * Automation:
   *
   */

  [ExportToolButton("Auto-Populate Profile Bindings")]
  private Callable AutoPopulateButton => Callable.From(AutoPopulateMissingBindings);

  private void AutoPopulateMissingBindings() {
    AllBindings ??= [];
    var missing = FindMissingActions();

    foreach (var action in missing)
      AllBindings.Add(CreateDefaultBinding(action));
  }


  /*
   *
   * Validation:
   * - does my input profile have a binding for all of the game's actions?
   * - do all my bindings have an assigned SourceEvent, and do these each match the same DeviceType?
   *
   *
   */

  public bool IsValid(out string error, string tabs = "") {
    var missing = FindMissingActions();

    var sb = new StringBuilder();
    if (missing.Count > 0) {
      sb.AppendLine($"{tabs}Invalid profile. Missing bindings:");
      foreach (var action in missing)
        sb.AppendLine($"{tabs}  - {action.ActionName} ({action.ValueType})");

      error = sb.ToString();
      return false;
    }

    DeviceTypeEnum? expectedDevice = null;
    foreach (var binding in AllBindings) {
      switch (binding) {
        case SingularInputBinding singular:
          if (singular.SourceEvent is null) {
            sb.AppendLine($"{tabs}  - '{singular.ActionName}' has no SourceEvent assigned.");
            continue;
          }

          var singularDevice = InputBinding.GetBindingDeviceType(singular);
          if (singularDevice == DeviceTypeEnum.Unsupported)
            sb.AppendLine($"{tabs}  - '{singular.ActionName}' has an unsupported input type.");
          else if (expectedDevice is null)
            expectedDevice = singularDevice;
          else if (singularDevice != expectedDevice)
            sb.AppendLine($"{tabs}  - '{singular.ActionName}': expected {expectedDevice}, got {singularDevice}.");
          break;

        case CompositeInputBinding composite:
          if (composite.Bindings is null) {
            sb.AppendLine($"{tabs}  - '{composite.ActionName}' has no child bindings.");
            break;
          }

          foreach (var child in composite.Bindings) {
            if (child is null) {
              sb.AppendLine($"{tabs}  - '{composite.ActionName}' has a null child binding.");
              continue;
            }

            if (child.SourceEvent is null) {
              sb.AppendLine($"{tabs}  - '{composite.ActionName}' has a child with no SourceEvent.");
              continue;
            }

            var childDevice = InputBinding.GetBindingDeviceType(child);
            if (childDevice == DeviceTypeEnum.Unsupported)
              sb.AppendLine($"{tabs}  - '{composite.ActionName}' has a child with an unsupported input type.");
            else if (expectedDevice is null)
              expectedDevice = childDevice;
            else if (childDevice != expectedDevice)
              sb.AppendLine($"{tabs}  - '{composite.ActionName}': expected {expectedDevice}, got {childDevice}.");
          }

          break;
      }
    }

    if (sb.Length > 0) {
      sb.Insert(0, $"{tabs}Invalid profile. Binding errors:\n");
      error = sb.ToString();
      return false;
    }

    error = null;
    return true;
  }


  /*
   *
   * Helpers
   *
   */

  private List<InputAction> FindMissingActions() {
    var bound = new HashSet<StringName>();
    if (AllBindings == null) return [];

    foreach (var binding in AllBindings) {
      if (binding?.ActionName is not null) {
        bound.Add(binding.ActionName);
        continue;
      }

      GD.PushWarning("A binding was found without an ActionName.");
    }

    var missing = new List<InputAction>();
    foreach (var action in InputActions.All.Values) {
      if (!bound.Contains(action.ActionName))
        missing.Add(action);
    }

    return missing;
  }

  private static InputBinding CreateDefaultBinding(InputAction action) {
    return action.ValueType switch {
      InputActionValueEnum.Bool => new SingularInputBinding(action.ActionName,
        new InputEventKey() { Keycode = Key.Space }),
      InputActionValueEnum.Axis1D => new SingularInputBinding(action.ActionName, new InputEventJoypadMotion()),
      InputActionValueEnum.Vector2 => GetDefault2D(action.ActionName),
      InputActionValueEnum.Vector3 => GetDefault3D(action.ActionName),
      _ => throw new ArgumentOutOfRangeException()
    };
  }

  private static CompositeInputBinding GetDefault2D(InputActionName actionName) => new(actionName, [
    new CompositeBindingChild(new InputEventKey() { Keycode = Key.W },
      [new SwizzleModifier(order: SwizzleModifier.SwizzleOrder.YXZ)]),

    new CompositeBindingChild(new InputEventKey() { Keycode = Key.A }, [new NegateModifier()]),

    new CompositeBindingChild(new InputEventKey() { Keycode = Key.S },
      [new SwizzleModifier(order: SwizzleModifier.SwizzleOrder.YXZ), new NegateModifier()]),

    new CompositeBindingChild(new InputEventKey() { Keycode = Key.D }, [])
  ]);

  private static CompositeInputBinding GetDefault3D(InputActionName actionName) => new(actionName, [
    new CompositeBindingChild(new InputEventKey() { Keycode = Key.W },
      [new SwizzleModifier(order: SwizzleModifier.SwizzleOrder.ZYX), new NegateModifier()]),

    new CompositeBindingChild(new InputEventKey() { Keycode = Key.A }, [new NegateModifier()]),

    new CompositeBindingChild(new InputEventKey() { Keycode = Key.S },
      [new SwizzleModifier(order: SwizzleModifier.SwizzleOrder.ZYX)]),

    new CompositeBindingChild(new InputEventKey() { Keycode = Key.D }, [])
  ]);
}