using Godot;

/// <summary>
/// Modifiers are stateless Variant transforms with type metadata. The metadata
/// declares what types each modifier accepts and what it outputs, purely for
/// editor validation and tooling. At runtime they just process Variants.
/// </summary>
public abstract partial class InputModifier : Resource {
  /// <summary>
  ///   Transform an input value. Return the modified value.
  /// </summary>
  /// <param name="input">Current (possibly already modified) value.</param>
  /// <param name="delta">Frame delta time.</param>
  public abstract InputPipelineData Process(InputPipelineData input, float delta);
}