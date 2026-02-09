using Godot;

public readonly struct InputModifierDebugContext {
  public StringName ActionName { get; init; }
  //future: DeviceId, MappingContext, etc.
}

public interface IInputModifier {
  /// <summary>
  ///   Transform an input value. Return the modified value.
  /// </summary>
  /// <param name="input">Current (possibly already modified) value.</param>
  /// <param name="delta">Frame delta time.</param>
  /// <param name="ctx">Debug data, primarily used for signaling invalid usage.</param>
  Variant Process(Variant input, float delta, in InputModifierDebugContext ctx);
}