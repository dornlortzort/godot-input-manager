/// <summary>
///   The lifecycle of an input with support for arbitrary trigger conditions.
///   None → Pending → Canceled -- or -- None → Pending → Activated → (Sustained) -> Completed
///   For instant triggers (press, flick), Pending is skipped entirely:
///   None → Activated → Completed
///   Pending = trigger is evaluating but hasn't resolved. (e.g. Game code for charge attacks binds here to show charge-up
///   VFX.)
///   Activated = trigger condition met. The "do the thing" moment (e.g. Jump, Dash, Fire).
///   Completed = input returned to rest *after* activation. Release logic, end of sustained fire, etc.
///   Canceled = input return to rest *before* activation. Cancel charge-up VFX, play fizzle animation, etc.
/// </summary>
public enum InputActionPhaseEnum {
  None,
  Pending,
  Canceled,
  Activated,
  Completed
}