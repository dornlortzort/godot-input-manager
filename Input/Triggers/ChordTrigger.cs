// using Godot;
// using System;
//
// chorded actions are pretty complex... require special manager consumption patterns so that prerequisite actions are all evaluated first (or last) and have a way to mark an action as "consumed this frame" so it doesn't also fire its own gameplay effect while it's being used as a chord modifier.
// public partial class ChordTrigger : InputTrigger
// {
//   [Export] public StringName RequiredAction { get; private set; }
//   
//   // The manager injects this reference when building the lookup
//   public IInputAction ResolvedAction { get; set; }
//   
//   public override InputPhase Evaluate(InputPipelineValue input, float delta, InputDebugContext ctx) {
//     if (ResolvedAction?.Phase is not InputPhase.Activated) {
//       return InputPhase.None;
//     }
//   }
//
//   public override void Reset() {
//     throw new NotImplementedException();
//   }
// }
