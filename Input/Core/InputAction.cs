using System;
using Godot;

/// <summary>
///   Generic abstract base for all input actions. Inherits from Resource
///   for Godot serialization. Implements IInputAction for polymorphic access.
///   NOT registered with Godot (i.e. [GlobalClass] bc Godot's type system
///   can't handle open generics. Instead the leaf classes, which are typed
///   (e.g. BoolInputAction) must be assigned [GlobalClass].
///   This layer provides:
///   - The typed ReceiveValue → ReceiveTypedValue pipeline
///   - Phase and elapsed time storage
///   - ValueType reflection
///   It deliberately does NOT provide a Value property. The getter for value
///   depends on the action type -- IReadableInput or IConsumableInput.
/// </summary>
public abstract partial class InputAction<TValue> : Resource, IInputAction where TValue : struct {
  [Signal]
  public delegate void CanceledEventHandler();

  [Signal]
  public delegate void CompletedEventHandler();

  [Signal]
  public delegate void PendingEventHandler();

  [Signal]
  public delegate void TriggeredEventHandler();

  // ** Design-time metadata ** 
  [Export] public StringName ActionName { get; private set; }
  [Export] public string DisplayName { get; private set; }
  [Export] public bool IsRemappable { get; private set; } = true;
  [Export] public float BufferSeconds { get; set; }

  // todo: add a default trigger (triggers will inherit Resource as well)
  //  type = NoTrigger/EmptyTrigger?

  // todo: Q: Is abstract correct here?
  public abstract Type ValueType { get; }

  // ** Runtime state (readonly to game code) **
  public InputPhase Phase { get; private set; } = InputPhase.None;
  public float PhaseStartTime { get; private set; }
  public float ElapsedSecondsInPhase => (float)(Time.GetTicksMsec() / 1000.0 - PhaseStartTime);
  public bool IsActive => Phase is InputPhase.Pending or InputPhase.Triggered;

  public void UpdatePhase(InputPhase phase) {
    if (phase == Phase) return;

    Phase = phase;
    PhaseStartTime = (float)(Time.GetTicksMsec() / 1000.0);
    switch (phase) {
      case InputPhase.None:
        break;
      case InputPhase.Pending:
        EmitSignal(SignalName.Pending);
        break;
      case InputPhase.Triggered:
        EmitSignal(SignalName.Triggered);
        break;
      case InputPhase.Canceled:
        EmitSignal(SignalName.Canceled);
        break;
      case InputPhase.Completed:
        EmitSignal(SignalName.Completed);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
    }
  }

  public abstract void ReceiveValue(TValue value, float delta);
}