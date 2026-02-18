using Godot;

public partial class ScaleModifier : InputModifier {
  [Export] public float Scale { get; private set; }

  public override InputPipelineData Process(InputPipelineData input, float delta) {
    input.Value *= Scale;
    return input;
  }
}