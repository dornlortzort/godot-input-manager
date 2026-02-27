using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class CompositeInputBinding : InputBinding {
  [Export] public Array<SimpleInputBinding> Bindings { get; private set; }
  [Export] public Array<InputModifier> Modifiers { get; private set; }

  private readonly List<InputSample> _samplesThisFrame = new(6);

  [Obsolete("Use parameterized constructor")]
  public CompositeInputBinding() {
  }

  public CompositeInputBinding(InputActionName actionName, Array<SimpleInputBinding> bindings,
    Array<InputModifier> modifiers = null)
    : base(actionName) {
    Bindings = bindings;
    Modifiers = modifiers ?? new();
  }

  public override void _ValidateProperty(Dictionary property) {
    ResourceName = GetResourceName();
  }

  public override string GetResourceName() =>
    $"{ActionName}: ({string.Join(", ", Bindings.Select(b => b.GetInputSourceName()))})";

  /*
   *
   * Api
   *
   */
  internal void ReceiveChildSample(InputSample sample) {
    _samplesThisFrame.Add(sample);
  }

  /// <summary>
  /// slightly more complex than a SimpleInputBinding's DrainTo() since this one has to
  /// first sum all its constituent Bindings' samples into one, then sum them all
  /// </summary>
  public override void DrainTo(InputAction action, double delta) {
    var span = CollectionsMarshal.AsSpan(_samplesThisFrame);
    for (int i = 0; i < span.Length; i++) {
      foreach (var modifier in Modifiers) {
        span[i] = modifier.Process(span[i]);
      }
    }

    action.Process(span, delta);
    _samplesThisFrame.Clear();
  }
}