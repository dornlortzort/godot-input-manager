using Godot;

/// <summary>
/// This is a DownTrigger
/// </summary>
public partial class DownTrigger : InputTrigger {
  [Export] public float Threshold { get; private set; } = 0.3f;
  private bool _isActive;

  public override InputActionPhaseEnum Evaluate(InputPipelineData input, float delta) {
    var magnitude = input.Value.Length();

    var isAbove = magnitude >= Threshold;
    switch (isAbove) {
      case true when !_isActive:
        _isActive = true;
        return InputActionPhaseEnum.Activated;
      case true when _isActive:
        return InputActionPhaseEnum.Activated;
      case false when _isActive:
        _isActive = false;
        return InputActionPhaseEnum.Completed;
      default:
        return InputActionPhaseEnum.None;
    }
  }

  public override void Reset() {
    _isActive = false;
  }
}