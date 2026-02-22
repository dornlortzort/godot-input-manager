/// <summary>
/// The lifecycle of an input with support for arbitrary trigger conditions.
/// For instant triggers (press, flick), Pending is skipped entirely:
/// Pending = trigger is evaluating but hasn't resolved. (e.g. Game code for charge attacks binds here to show charge-up
/// VFX.)
/// Activated = trigger condition met. The "do the thing" moment (e.g. Jump, Dash, Fire).
/// InputAction can report Canceled or Completed when Pending -> None or Activated -> None reported.
/// </summary>
public enum InputActionPhaseEnum {
  None,
  Pending,
  Activated,
}