using Godot;

/// <summary>
/// Modifiers are stateless Variant transforms with type metadata. The metadata
/// declares what types each modifier accepts and what it outputs, purely for
/// editor validation and tooling. At runtime they just process Variants.
///
/// Conventions:
/// - Build a parameterized constructor wherever possible.
/// </summary>
[Tool]
[GlobalClass]
public abstract partial class InputModifier : Resource {
  /// <summary>
  ///   Transform an input value. Return the modified value.
  /// </summary>
  /// <param name="input">Current (possibly already modified) value.</param>
  public abstract InputPayload Process(InputPayload input);
}