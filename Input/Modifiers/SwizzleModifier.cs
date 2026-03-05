using System;
using Godot;

[Tool]
[GlobalClass]
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

  [Obsolete("Use parameterized constructor")]
  public SwizzleModifier() {
  }

  public SwizzleModifier(SwizzleOrder order) {
    Order = order;
  }

  public override InputPayload Process(InputPayload input) => Order switch {
    SwizzleOrder.XYZ => input,
    SwizzleOrder.YXZ => new(input.Y, input.X, input.Z),
    SwizzleOrder.XZY => new(input.X, input.Z, input.Y),
    SwizzleOrder.YZX => new(input.Y, input.Z, input.X),
    SwizzleOrder.ZXY => new(input.Z, input.X, input.Y),
    SwizzleOrder.ZYX => new(input.Z, input.Y, input.X),
    _ => input
  };
}