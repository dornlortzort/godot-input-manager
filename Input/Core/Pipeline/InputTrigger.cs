using System;
using Godot;

/// <summary>
/// Triggers are stateful phase evaluators owned by their InputAction. They hold whatever
/// internal state they need (hold timers, tap windows) and that state is scoped to
/// the action — no shared mutable state across the system.
///
/// Conventions:
/// - implement AsCodeDeclarationString() so that this can compile to static
///   types via the InputRegistry's generator.
/// </summary>
[Tool]
[GlobalClass]
public abstract partial class InputTrigger : Resource, ICustomNamedResource {
  public abstract string AsCodeDeclarationString();
  public abstract string GetResourceName();

  /// <summary>
  ///   Evaluate the current input and return what phase the InputAction should be in
  ///   as a result of this value.
  /// </summary>
  public abstract InputActionPhaseEnum Evaluate(
    ReadOnlySpan<InputPayload> payloadsThisFrame, double delta);

  protected abstract InputActionPhaseEnum EvaluateSample(InputPayload payload);

  /// <summary>
  ///   Called when the system's InputMode or InputProfile changes
  /// </summary>
  public abstract void Reset();

  /// <summary>
  /// Since InputTriggers are stateful we want 
  /// </summary>
  public abstract InputTrigger Clone();
}