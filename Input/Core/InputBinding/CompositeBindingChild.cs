using Godot;
using Godot.Collections;
using System;

/// <summary>
/// This isn't a formal InputBinding type because we don't want it to be selectable in an
/// InputProfile's Bindings field from the editor. It should only ever exist as a child to
/// CompositeInputBinding.
///
/// Implements IEventCapturableBinding so it can be polymorphic with SingularInputBinding
/// since both are used to listen to incoming events in the InputSystem
/// </summary>
[Tool]
[GlobalClass]
public partial class CompositeBindingChild : Resource, IEventCapturableBinding, ICustomNamedResource {
  [Export] public InputEvent SourceEvent { get; private set; }
  [Export] private Array<InputModifier> _modifiers;

  /// <summary>
  /// Assigned upon CompiledInputProfile creation, which happens when the
  /// InputSystem calls ApplyProfile()
  /// </summary>
  internal CompositeInputBinding Parent { get; set; }

  public InputActionName ActionName => Parent.ActionName;

  /// <summary>
  /// Persistent state between each frame. Parent reads from this.
  /// </summary>
  public InputPayload CurrentState { get; private set; }

  [Obsolete("Use parameterized constructor")]
  protected CompositeBindingChild() {
  }

  public CompositeBindingChild(InputEvent sourceEvent,
    Array<InputModifier> modifiers = null) {
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

    CurrentState = payload;
    Parent.ReceiveChildPayload(payload);
  }

  /*
   *
   * Tooling
   *
   */
  public override void _ValidateProperty(Dictionary property) {
    ResourceName = GetResourceName();
  }

  public string GetResourceName() => InputBinding.GetBindingSourceName(this);
}