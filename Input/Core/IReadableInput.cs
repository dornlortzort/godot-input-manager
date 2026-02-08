public interface IReadableInput<TValue> where TValue : struct {
  TValue Value { get; }
}