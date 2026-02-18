public abstract partial class ContinuousInputAction<TValue> : BaseInputAction where TValue : struct {
  public abstract TValue Value { get; }

  public override void ReceiveValue(InputPipelineData input) {
    _currentValue = input.Value;
  }
}