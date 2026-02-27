using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class SimpleInputBinding : InputBinding {
  [Export] public InputEvent SourceEvent { get; private set; }
  [Export] public Array<InputModifier> Modifiers { get; private set; }

  /// <summary>
  /// Kind of a tricky detail: this gets assigned when an InputProfile builds into
  /// a CompiledInputProfile for the manager.
  /// </summary>
  internal CompositeInputBinding Parent { get; set; }

  /// <summary>
  /// This is a queue, as events come in for a frame they are added in order.
  /// </summary>
  private readonly List<InputSample> _samplesThisFrame = new(3);


  [Obsolete("Use parameterized constructor")]
  protected SimpleInputBinding() {
  }

  public SimpleInputBinding(InputActionName actionName, InputEvent sourceEvent, Array<InputModifier> modifiers = null)
    : base(actionName) {
    SourceEvent = sourceEvent;
    Modifiers = modifiers ?? new();
  }

  /*
   *
   * Api
   *
   */

  /// <summary>
  /// Captures an incoming <see cref="InputEvent"/>, converts it to an
  /// <see cref="InputSample"/>, runs it through this binding's modifier
  /// chain, and enqueues the result for processing.
  /// </summary>
  public void CaptureEvent(InputEvent e) {
    if (InputSample.From(ActionName, e) is not { } data) return;
    foreach (var modifier in Modifiers)
      data = modifier.Process(data);

    if (Parent is not null)
      Parent.ReceiveChildSample(data);
    else
      _samplesThisFrame.Add(data);
  }

  /// <summary>
  /// Passes all queued samples to the given <see cref="InputAction"/> in
  /// capture order, then clears the queue. The samples are provided as a
  /// span into the internal buffer and must be consumed synchronously
  /// within <see cref="InputAction.Process"/>.
  /// </summary>
  public override void DrainTo(InputAction action, double delta) {
    action.Process(CollectionsMarshal.AsSpan(_samplesThisFrame), delta);
    _samplesThisFrame.Clear();
  }

  /*
   *
   * Tooling:
   *
   */

  /// <summary>
  /// Ensures that the assignment of an ActionName is from the list of game actions defined
  /// in the registry. Helps reduce error-prone string typos and just makes for a better ux.
  /// </summary>
  public override void _ValidateProperty(Dictionary property) {
    if (property["name"].AsStringName() != InputBinding.PropertyName.ActionName) return;
    property["hint"] = (int)PropertyHint.Enum;
    property["hint_string"] = string.Join(",", InputActions.All.Keys);

    ResourceName = GetResourceName();
  }


  public override string GetResourceName() => $"{ActionName}: {GetInputSourceName()}";

  public string GetInputSourceName() => SourceEvent switch {
    InputEventKey key => key.PhysicalKeycode != Key.None
      ? key.PhysicalKeycode.ToString()
      : key.Keycode.ToString(),
    InputEventMouseButton mb => mb.ButtonIndex.ToString(),
    InputEventJoypadButton jb => jb.ButtonIndex.ToString(),
    InputEventJoypadMotion jm => $"Axis{jm.Axis}",
    _ => SourceEvent?.GetType().Name ?? "Unset"
  };
}