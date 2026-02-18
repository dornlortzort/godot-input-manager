using Godot;

public partial class PressTrigger : InputTrigger {
  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float Threshold { get; private set; } = 0.5f;

  private bool _wasAbove;

  public override InputActionPhaseEnum Evaluate(InputPipelineData input, float delta) {
    var magnitude = input.Value.Length();

    var isAbove = magnitude >= Threshold;
    InputActionPhaseEnum result;

    if (isAbove && !_wasAbove)
      result = InputActionPhaseEnum.Activated;
    else
      result = InputActionPhaseEnum.None;

    _wasAbove = isAbove;
    return result;
  }

  public override void Reset() {
    _wasAbove = false;
  }
}