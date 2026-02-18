using Godot;

/// <summary>
/// Triggers are stateful phase evaluators owned by their binding. They hold whatever
/// internal state they need (hold timers, tap windows) and that state is scoped to
/// the binding — no shared mutable state across the system.
/// </summary>
public abstract partial class InputTrigger : Resource {
  /// <summary>
  ///   Evaluate the current input and return what phase the InputAction should be in
  ///   as a result of this value.
  /// </summary>
  public abstract InputActionPhaseEnum Evaluate(InputPipelineData input, float delta);

  /// <summary>
  ///   Called when:
  ///   - a binding's mode becomes inactive
  ///   - player switches their InputProfile
  ///   - the manager needs a clean state
  /// </summary>
  public abstract void Reset();
}