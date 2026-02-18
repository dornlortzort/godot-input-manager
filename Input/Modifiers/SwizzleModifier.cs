using Godot;


public partial class SwizzleModifier : InputModifier {
  public enum SwizzleOrder {
    XYZ, // identity, no change
    YXZ,
    XZY,
    YZX,
    ZXY,
    ZYX,
  }

  [Export]
  public SwizzleOrder Order { get; private set; } =
    SwizzleOrder.XYZ;

  public override InputPipelineData Process(InputPipelineData input, float delta) {
    var v = input.Value;
    input.Value = Order switch {
      SwizzleOrder.XYZ => v,
      SwizzleOrder.YXZ => new(v.Y, v.X, v.Z),
      SwizzleOrder.XZY => new(v.X, v.Z, v.Y),
      SwizzleOrder.YZX => new(v.Y, v.Z, v.X),
      SwizzleOrder.ZXY => new(v.Z, v.X, v.Y),
      SwizzleOrder.ZYX => new(v.Z, v.Y, v.X),
      _ => v
    };
    return input;
  }
}