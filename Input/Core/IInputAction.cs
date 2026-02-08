using System;
using Godot;

public interface IInputAction {
  StringName ActionName { get; }
  string DisplayName { get; }
  bool IsRemappable { get; }

  /// Buffer window in ms, 0 = no buffer
  float BufferSeconds { get; }

  /// <summary>
  ///   The System.Type of the value this action works with.
  ///   Useful for debug tooling and validation.
  /// </summary>
  Type ValueType { get; }

  InputPhase Phase { get; }
  public float PhaseStartTime { get; }
  public float ElapsedSecondsInPhase { get; }
  bool IsActive { get; } // convenience: true when Pending or Activated
  void UpdatePhase(InputPhase phase);
}