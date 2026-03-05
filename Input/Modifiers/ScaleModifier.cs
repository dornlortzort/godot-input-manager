using System;
using Godot;

[Tool]
[GlobalClass]
public partial class ScaleModifier : InputModifier {
  [Export] public float Scale { get; private set; } = 1;

  [Obsolete("Use parameterized constructor")]
  public ScaleModifier() {
  }

  public ScaleModifier(float scale) {
    Scale = scale;
  }

  public override InputPayload Process(InputPayload input) => input * Scale;
}