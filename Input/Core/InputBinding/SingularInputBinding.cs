using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Implements IEventCapturableBinding so it can be polymorphic with CompositeBindingChild
/// since both are used to listen to incoming events in the InputSystem
/// </summary>
[Tool]
[GlobalClass]
public partial class SingularInputBinding : InputBinding, IEventCapturableBinding {
  [Export] private InputActionName _actionName;
  [Export] public InputEvent SourceEvent { get; private set; }
  [Export] private Array<InputModifier> _modifiers = [];

  public override InputActionName ActionName => _actionName;

  /// <summary>
  /// Chronological list of payloads captured this frame. Only used in standalone mode —
  /// when parented, payloads are forwarded to the composite instead.
  /// </summary>
  private readonly List<InputPayload> _payloadsThisFrame = new(3);

  [Obsolete("Use parameterized constructor")]
  protected SingularInputBinding() {
  }

  public SingularInputBinding(InputActionName actionName, InputEvent sourceEvent,
    Array<InputModifier> modifiers = null) {
    _actionName = actionName;
    SourceEvent = sourceEvent;
    _modifiers = modifiers ?? [];
  }

  public void CaptureEvent(InputEvent e) {
    GD.Print($"CaptureEvent() for action {ActionName}");
    if (InputPayload.From(e) is not { } payload) return;
    GD.Print($"\tConstructed InputPayload.From(e) as {payload}");
    foreach (var modifier in _modifiers) {
      payload = modifier.Process(payload);
      GD.Print($"\tModified into {payload}");
    }

    _payloadsThisFrame.Add(payload);
  }

  /// <summary>
  /// Passes all queued payloads to the given <see cref="InputAction"/> in
  /// capture order, then clears the queue. Only used in standalone mode —
  /// parented bindings drain through their composite.
  /// </summary>
  public override void DrainTo(InputAction action, double delta) {
    if (_payloadsThisFrame.Count > 0)
      GD.Print($"Binding {GetResourceName()} DrainTo() called for action {ActionName}");

    action.Process(CollectionsMarshal.AsSpan(_payloadsThisFrame), delta);
    _payloadsThisFrame.Clear();
  }

  /*
   *
   * Tooling
   *
   */
  public override void _ValidateProperty(Dictionary property) {
    ResourceName = GetResourceName();
  }

  public override string GetResourceName() => $"{ActionName}: {GetBindingSourceName(this)}";
}