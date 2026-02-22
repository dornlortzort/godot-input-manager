using System;
using Godot;

/// <summary>
/// Actions are pure destinations. Name, value, phase, signals.
/// They don't own any logic for how they get activated (this is the binding's job
/// instead, with modifiers and triggers. Actions are the public API of the input
/// system. Game code subscribes to their signals or reads their state. 
/// </summary>
public abstract partial class InputAction : GodotObject {
  [Signal]
  public delegate void OnCanceledEventHandler();

  [Signal]
  public delegate void OnCompletedEventHandler();

  [Signal]
  public delegate void OnPendingEventHandler();

  [Signal]
  public delegate void OnActivatedEventHandler();


  ///
  /// Trigger Logic
  ///
  private InputTrigger _trigger;

  private DownTrigger _defaultTrigger = new();

  public InputTrigger Trigger {
    get => _trigger ?? _defaultTrigger;
    set => _trigger = value;
  }


  // These get set directly by generated code
  public StringName ActionName { get; init; }
  public string DisplayName { get; init; }
  public InputActionValueEnum ValueType { get; init; }
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

  public virtual void Process(ReadOnlySpan<InputSample> samplesThisFrame, float delta) {
    var resultPhase = Trigger.Evaluate(samplesThisFrame, delta);
    ConsumeSamples(samplesThisFrame);

    if (resultPhase != Phase) {
      PhaseStartTime = (float)(Time.GetTicksMsec() / 1000.0);
      switch (resultPhase) {
        case InputActionPhaseEnum.None:
          if (Phase == InputActionPhaseEnum.Pending)
            EmitSignalOnCanceled();
          if (Phase == InputActionPhaseEnum.Activated)
            EmitSignalOnCompleted();
          break;
        case InputActionPhaseEnum.Pending:
          EmitSignalOnPending();
          break;
        case InputActionPhaseEnum.Activated:
          EmitSignalOnActivated();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(resultPhase), resultPhase, null);
      }

      Phase = resultPhase;
    }
  }

  protected abstract void ConsumeSamples(ReadOnlySpan<InputSample> samplesThisFrame);
}