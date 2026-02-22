using Godot;

[GlobalClass]
public partial class NegateModifier : InputModifier {
  public override InputSample Process(InputSample input) {
    input.Value *= -1;
    return input;
  }

  public NegateModifier() {
  }
}