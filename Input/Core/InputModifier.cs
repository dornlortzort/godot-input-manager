using Godot;

public abstract partial class InputModifier : Resource, IInputModifier {
  public abstract Variant Process(Variant input, float delta, in InputDebugContext ctx);
}