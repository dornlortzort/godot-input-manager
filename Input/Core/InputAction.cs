using System;
using Godot;

/// <summary>
///   Generic abstract base for all input actions. Inherits from Resource
///   for Godot serialization. Implements IInputAction for polymorphic access.
///   NOT registered with Godot (i.e. [GlobalClass]) bc Godot's type system
///   can't handle open generics. Instead the leaf classes, which are typed
///   (e.g. BoolInputAction) must be assigned [GlobalClass].
///   This layer provides:
///   - a Value getter. For standard continuous inputs, this is just the current
///   value on the frame. For delta inputs, it's the value accumulated across
///   input events since the last frame, which then gets cleared at end-of-frame
///   by the manager.
///   - The typed ReceiveValue → ReceiveTypedValue pipeline
///   - Phase and elapsed time storage
///   - ValueType reflection
/// </summary>
public abstract partial class InputAction<[MustBeVariant] TValue> : Resource, IInputAction where TValue : struct {
  [Signal]
  public delegate void CanceledEventHandler();

  [Signal]
  public delegate void CompletedEventHandler();

  [Signal]
  public delegate void PendingEventHandler();

  [Signal]
  public delegate void TriggeredEventHandler();

  [Signal]
  public delegate void SustainedEventHandler();

  public abstract TValue Value { get; protected set; }
  public abstract Type ValueType { get; }


  // ** Design-time metadata ** 
  [Export] public StringName ActionName { get; private set; }
  [Export] public string DisplayName { get; private set; }
  [Export] public bool IsRemappable { get; private set; } = true;
  [Export] public float BufferSeconds { get; set; }

  // abstract InputTrigger type, exportable for editor config
  [Export] public InputTrigger Trigger { get; private set; }

  // satisfies the interface with explicit implementation
  IInputTrigger IInputAction.Trigger => EffectiveTrigger;
  public IInputTrigger EffectiveTrigger => Trigger ?? _defaultTrigger;
  private readonly DownTrigger _defaultTrigger = new();

  // ** Runtime state (readonly to game code) **
  public InputPhase Phase { get; private set; } = InputPhase.None;
  public float PhaseStartTime { get; private set; }
  public float ElapsedSecondsInPhase => (float)(Time.GetTicksMsec() / 1000.0 - PhaseStartTime);
  public bool IsActive => Phase is InputPhase.Pending or InputPhase.Triggered;

  /// <summary>
  ///   Receive a Variant value, because the godot engine passes Variants from
  ///   its raw inputs
  /// </summary>
  public void ReceiveValueAndPhaseUpdates(Variant value, InputPhase phase) {
    ReceiveTypedValue(value.As<TValue>());

    if (phase == Phase) return;

    Phase = phase;
    PhaseStartTime = (float)(Time.GetTicksMsec() / 1000.0);
    switch (phase) {
      case InputPhase.None:
        break;
      case InputPhase.Pending:
        EmitSignalPending();
        break;
      case InputPhase.Canceled:
        EmitSignalCanceled();
        break;
      case InputPhase.Triggered:
        EmitSignalTriggered();
        break;
      case InputPhase.Sustained:
        EmitSignalSustained();
        break;
      case InputPhase.Completed:
        EmitSignalCompleted();
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
    }
  }

  public abstract void ReceiveTypedValue(TValue value);
}

/*
 * Notes:
 * [MustBeVariant] enforces at compile-time that TValue must be one of the Variant types. Makes it trivially easy to implement
 * ReceiveValue using the As method.
 *  - Variant docs:
 *    - C# Variant: https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_variant.html
 *    - compatible types: https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_variant.html#variant-compatible-types
 * On Godot source generators:
 * - Things like [MustBeVariant], [Export], [Signal], etc. run during a build and generate "bridge code" to connect our C# code
 * to the native engine. For this bridge code to work, that's why we add the keyword `partial` to our class.
 */