public interface IConsumableInput<TValue> where TValue : struct {
  TValue Consume();
}