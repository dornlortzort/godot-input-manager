public interface IDeltaInput<TValue> where TValue : struct {
  void ResetAccumulator();
}