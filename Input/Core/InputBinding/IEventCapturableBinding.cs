using Godot;

/// <summary>
/// A simple interface for SingularInputBinding and CompositeBindingChild to implement
/// </summary>
public interface IEventCapturableBinding {
  InputEvent SourceEvent { get; }
  InputActionName ActionName { get; }

  /// <summary>
  /// Captures an incoming <see cref="InputEvent"/>, converts it to an
  /// <see cref="InputPayload"/>, runs it through this binding's modifier
  /// chain, stores it as current state, and either forwards to the parent
  /// composite (if CompositeBindingChild) or enqueues locally (if
  /// SingularInputBinding).
  /// </summary>
  void CaptureEvent(InputEvent e);
}