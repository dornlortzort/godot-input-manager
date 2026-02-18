using Godot;

public static class InputActionUtils {
}

[GlobalClass]
public partial class BoolInputAction : ContinuousInputAction<bool> {
  public override bool Value => AsBool(_currentValue);
}

[GlobalClass]
public partial class FloatInputAction : ContinuousInputAction<float> {
  public override float Value => AsFloat(_currentValue);
}

[GlobalClass]
public partial class Vector2InputAction : ContinuousInputAction<Vector2> {
  public override Vector2 Value => AsVector2(_currentValue);
}

[GlobalClass]
public partial class Vector3InputAction : ContinuousInputAction<Vector3> {
  public override Vector3 Value => _currentValue;
}

/// 
/// DELTA INPUTS
///
[GlobalClass]
public partial class DeltaBoolInputAction : DeltaInputAction<bool> {
  public override bool FrameDelta => AsBool(_currentValue);
}

[GlobalClass]
public partial class DeltaFloatInputAction : DeltaInputAction<float> {
  public override float FrameDelta => AsFloat(_currentValue);
}

[GlobalClass]
public partial class DeltaVector2InputAction : DeltaInputAction<Vector2> {
  public override Vector2 FrameDelta => AsVector2(_currentValue);
}

[GlobalClass]
public partial class DeltaVector3InputAction : DeltaInputAction<Vector3> {
  public override Vector3 FrameDelta => _currentValue;
}