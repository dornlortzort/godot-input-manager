using Godot;

public partial class DeadZoneModifier : InputModifier {
  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float InnerThreshold { get; private set; } = 0.2f;

  [Export(PropertyHint.Range, "0.0,1.0,0.01")]
  public float OuterThreshold { get; private set; } = 0.95f;

  public override InputPipelineData Process(InputPipelineData input, float delta) {
    if (OuterThreshold <= InnerThreshold) {
      input.Value = Vector3.Zero;
      return input;
    }

    var magnitude = input.Value.Length();
    if (magnitude < InnerThreshold) {
      input.Value = Vector3.Zero;
      return input;
    }

    var remappedMagnitude = Mathf.Clamp(Mathf.InverseLerp(InnerThreshold, OuterThreshold, magnitude), 0f, 1f);
    input.Value = input.Value.Normalized() * remappedMagnitude;
    return input;
  }
}

/*
Notes:
Lerp = (t) => a value between a and b
so, InverseLerp = (a value between a and b) => t

Meaning this function will proportionalize the provided input as if
the lower and upper bound are 0 and 1.
*/