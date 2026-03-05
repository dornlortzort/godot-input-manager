using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using Godot.Collections;

public enum CompositeStrategy {
  HighWaterMark,
  MostRecent,
  Sum
}

[Tool]
[GlobalClass]
public partial class CompositeInputBinding : InputBinding {
  [Export] private InputActionName _actionName;
  [Export] public Array<CompositeBindingChild> Bindings { get; private set; }
  [Export] public Array<InputModifier> Modifiers { get; private set; }

  [ExportGroup("Advanced")]
  [Export]
  public CompositeStrategy Strategy { get; private set; } = CompositeStrategy.HighWaterMark;

  public override InputActionName ActionName => _actionName;

  /// <summary>
  /// Chronological list of combined snapshots this frame. Each child event produces
  /// one entry, so triggers see every state transition.
  /// </summary>
  private readonly List<InputPayload> _payloadsThisFrame = new(6);

  [Obsolete("Use parameterized constructor")]
  public CompositeInputBinding() {
  }

  public CompositeInputBinding(InputActionName actionName, Array<CompositeBindingChild> bindings,
    Array<InputModifier> modifiers = null,
    CompositeStrategy strategy = CompositeStrategy.HighWaterMark) {
    _actionName = actionName;
    Bindings = bindings;
    Modifiers = modifiers ?? new();
    Strategy = strategy;
  }


  /*
   *
   * API
   *
   */

  /// <summary>
  /// Passes the full chronological sequence of combined snapshots to the action.
  /// The action and its triggers see every state transition that occurred this frame,
  /// identical contract to a standalone SimpleInputBinding.
  /// </summary>
  public override void DrainTo(InputAction action, double delta) {
    if (_payloadsThisFrame.Count > 0)
      GD.Print($"Binding {GetResourceName()} DrainTo() called for action {ActionName}");


    var span = CollectionsMarshal.AsSpan(_payloadsThisFrame);
    for (var i = 0; i < span.Length; i++)
      foreach (var modifier in Modifiers)
        span[i] = modifier.Process(span[i]);

    action.Process(span, delta);
    _payloadsThisFrame.Clear();
  }

  /*
   *
   * Receiving child events
   *
   */

  /// <summary>
  /// Called by a child whenever it captures an event. Recomputes the value
  /// using the active strategy, applies composite-level modifiers, and
  /// appends the result to this frame's payload list.
  /// </summary>
  internal void ReceiveChildPayload(InputPayload payload) {
    var combined = Strategy switch {
      CompositeStrategy.HighWaterMark => ReceiveUsingHighWaterMark(),
      CompositeStrategy.MostRecent => ReceiveUsingMostRecent(payload),
      CompositeStrategy.Sum => CombineSum(),
      _ => throw new ArgumentOutOfRangeException()
    };

    foreach (var modifier in Modifiers)
      combined = modifier.Process(combined);

    GD.Print($"ReceiveChildPayload() resulted in {payload}");
    _payloadsThisFrame.Add(combined);
  }

  /*
   *
   * API Helpers
   *
   */
  /// <summary>
  /// Per channel, the child with the highest absolute value wins.
  ///
  /// When W (Y=1) and S (Y=-1) are both held, the one pressed harder (or, for digital inputs,
  /// the one with magnitude 1 vs 0) wins. When one releases, the other naturally takes over
  /// because we recompute from current child states.
  ///
  /// Example frame — two children on the X axis:
  ///   A fires X=0.8  → states: [0.8, 0  ] → max abs → X=0.8
  ///   B fires X=0.5  → states: [0.8, 0.5] → max abs → X=0.8  (A still wins)
  ///   A releases X=0  → states: [0,   0.5] → max abs → X=0.5  (B takes over)
  ///
  /// In cases where magnitude is a tie, give it to the more recent of the two. (achieved w `>=`) 
  /// </summary>
  private InputPayload ReceiveUsingHighWaterMark() {
    float? x = null, y = null, z = null;
    foreach (var child in Bindings) {
      if (child.CurrentState.X.HasValue && (!x.HasValue || Math.Abs(child.CurrentState.X.Value) >= Math.Abs(x.Value)))
        x = child.CurrentState.X.Value;
      if (child.CurrentState.Y.HasValue && (!y.HasValue || Math.Abs(child.CurrentState.Y.Value) >= Math.Abs(y.Value)))
        y = child.CurrentState.Y.Value;
      if (child.CurrentState.Z.HasValue && (!z.HasValue || Math.Abs(child.CurrentState.Z.Value) >= Math.Abs(z.Value)))
        z = child.CurrentState.Z.Value;
    }

    return new(x, y, z);
  }

  /// <summary>
  /// Per channel, the most recently fired child's value wins.
  ///
  /// Unlike the other two strategies, this one doesn't need to scan all
  /// children. The incoming payload IS the most recent event, so we just
  /// layer it on top of the previous snapshot — any channel the new payload
  /// declares overwrites, everything else carries forward.
  ///
  /// Example frame — two children on the X axis:
  ///   A fires X=0.8  → prev: (null)       → snapshot: X=0.8
  ///   B fires X=0.5  → prev: X=0.8        → snapshot: X=0.5  (B overwrites)
  ///   A fires X=0    → prev: X=0.5        → snapshot: X=0    (A overwrites)
  ///
  /// This is useful when inputs should override each other rather than
  /// coexist — e.g. two conflicting control sources where the last touched
  /// one should take priority.
  /// </summary>
  private InputPayload ReceiveUsingMostRecent(InputPayload incoming) {
    var prev = _payloadsThisFrame.Count > 0
      ? _payloadsThisFrame[^1]
      : incoming;

    return new(incoming.X ?? prev.X, incoming.Y ?? prev.Y, incoming.Z ?? prev.Z);
  }

  /// <summary>
  /// Per channel, all children's current values are summed.
  ///
  /// Only non-null channels participate — a child that doesn't declare a
  /// channel contributes nothing rather than adding zero. The result channel
  /// itself stays null if no child has a value for it.
  ///
  /// Example frame — two analog children on X:
  ///   A fires X=0.6  → states: [0.6, 0  ] → sum → X=0.6
  ///   B fires X=0.4  → states: [0.6, 0.4] → sum → X=1.0
  ///   A fires X=0.2  → states: [0.2, 0.4] → sum → X=0.6
  ///
  /// Useful for analog inputs that should stack — dual throttle controls,
  /// multiple force feedback sources, etc. You'll typically want a clamp
  /// modifier on the composite to cap the combined range.
  /// </summary>
  private InputPayload CombineSum() {
    float? x = null, y = null, z = null;
    foreach (var child in Bindings) {
      if (child.CurrentState.X.HasValue) x = (x ?? 0f) + child.CurrentState.X.Value;
      if (child.CurrentState.Y.HasValue) y = (y ?? 0f) + child.CurrentState.Y.Value;
      if (child.CurrentState.Z.HasValue) z = (z ?? 0f) + child.CurrentState.Z.Value;
    }

    return new(x, y, z);
  }

  /*
   *
   * Tooling
   *
   */

  public override void _ValidateProperty(Dictionary property) {
    ResourceName = GetResourceName();
  }

  public override string GetResourceName() =>
    $"{ActionName}: ({string.Join(", ", Bindings.Select(GetBindingSourceName))})";
}