using Godot;

[Tool]
[GlobalClass]
public partial class NegateModifier : InputModifier {
  public override InputPayload Process(InputPayload input) => -input;

  public NegateModifier() {
  }
}