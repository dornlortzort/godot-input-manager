using System;
using Godot;

/// <summary>
/// Actions are pure destinations. Name, value, phase, signals.
/// They don't own any logic for how they get activated (this is the binding's job
/// instead, with modifiers and triggers. Actions are the public API of the input
/// system. Game code subscribes to their signals or reads their state. 
/// </summary>
public abstract partial class BaseInputAction : GodotObject {
  [Signal]
  public delegate void CanceledEventHandler();

  [Signal]
  public delegate void CompletedEventHandler();

  [Signal]
  public delegate void PendingEventHandler();

  [Signal]
  public delegate void ActivatedEventHandler();


  // These get set directly by generated code
  public StringName ActionName { get; init; }
  public string DisplayName { get; init; }
  public bool IsRemappable { get; init; }
  public float BufferSeconds { get; init; }

  /// <summary>
  /// Typed as Vector3 since this is what the InputPipelineData carries along.
  /// Subclasses are generically typed and transform the Vector3 to its expected
  /// relative type.
  /// </summary>
  protected Vector3 _currentValue;

  public static bool AsBool(Vector3 v) => v.X > 0.5f;
  public static float AsFloat(Vector3 v) => v.X;
  public static Vector2 AsVector2(Vector3 v) => new(v.X, v.Y);

  public InputActionPhaseEnum Phase { get; private set; } = InputActionPhaseEnum.None;
  public float PhaseStartTime { get; private set; }
  public float ElapsedSecondsInPhase => (float)(Time.GetTicksMsec() / 1000.0 - PhaseStartTime);

  public virtual void ReceiveManagerUpdate(InputPipelineData data, InputActionPhaseEnum phaseThisFrame) {
    ReceiveValue(data);
    if (phaseThisFrame == Phase) return;

    Phase = phaseThisFrame;
    PhaseStartTime = (float)(Time.GetTicksMsec() / 1000.0);
    switch (phaseThisFrame) {
      case InputActionPhaseEnum.None:
        break;
      case InputActionPhaseEnum.Pending:
        EmitSignalPending();
        break;
      case InputActionPhaseEnum.Canceled:
        EmitSignalCanceled();
        break;
      case InputActionPhaseEnum.Activated:
        EmitSignalActivated();
        break;
      case InputActionPhaseEnum.Completed:
        EmitSignalCompleted();
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(phaseThisFrame), phaseThisFrame, null);
    }
  }

  public abstract void ReceiveValue(InputPipelineData input);
}