using System;
using Godot;

[GlobalClass]
public partial class ScaleModifier : InputModifier {
  [Export] public float Scale { get; private set; }

  [Obsolete("Use parameterized constructor")]
  public ScaleModifier() {
  }

  public ScaleModifier(float scale) {
    Scale = scale;
  }

  public override InputSample Process(InputSample input) {
    input.Value *= Scale;
    return input;
  }
}