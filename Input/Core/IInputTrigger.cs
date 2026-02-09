using Godot;

public interface IInputTrigger {
  /// <summary>
  ///   Evaluate the current input and return what phase the InputAction should be in
  ///   as a result of this value.
  /// </summary>
  InputPhase Evaluate(Variant input, float delta, InputDebugContext ctx);

  /// <summary>
  ///   Called when:
  ///   - a binding's mode becomes inactive
  ///   - player switches their InputProfile
  ///   - the manager needs a clean state
  /// </summary>
  void Reset();
}