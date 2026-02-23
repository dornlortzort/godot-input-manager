using System;
using Godot;

/// <summary>
/// Triggers are stateful phase evaluators owned by their InputAction. They hold whatever
/// internal state they need (hold timers, tap windows) and that state is scoped to
/// the action — no shared mutable state across the system.
///
/// Conventions:
/// - todo: always make a constructor, implement parameterized when possible
/// - todo: always implement AsCodeDeclaration
/// </summary>
public abstract partial class InputTrigger : Resource {
  /// <summary>
  ///   Evaluate the current input and return what phase the InputAction should be in
  ///   as a result of this value.
  /// </summary>
  public abstract InputActionPhaseEnum Evaluate(
    ReadOnlySpan<InputSample> samplesThisFrame, float delta);


  protected abstract InputActionPhaseEnum EvaluateSample(InputSample sample);

  /// <summary>
  ///   Called when the manager's InputMode or InputProfile changes
  /// </summary>
  public abstract InputTrigger Clone();

  public abstract void Reset();
  public abstract string AsCodeDeclarationString();
}