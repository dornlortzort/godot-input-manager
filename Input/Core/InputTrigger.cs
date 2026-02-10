using Godot;

public abstract partial class InputTrigger : Resource, IInputTrigger {
  public abstract InputPhase Evaluate(Variant input, float delta, InputDebugContext ctx);
  public abstract void Reset();
}