using System;
using System.Collections.Generic;

namespace Cryville.Input {
	/// <summary>
	/// A simple input consumer that receives input frames.
	/// </summary>
	/// <param name="manager">The input manager.</param>
	public class SimpleInputConsumer(InputManager manager) {
		readonly object _lock = new();
		readonly Dictionary<InputIdentifier, InputFrame> _frames = [];
		readonly List<InputEvent> _events = [];

		/// <summary>
		/// Activates all the input handlers.
		/// </summary>
		public void Activate() {
			lock (_lock) {
				_events.Clear();
			}
			manager.EnumerateHandlers(h => h.OnInput += OnInput);
		}
		/// <summary>
		/// Deactivates all the input handlers.
		/// </summary>
		public void Deactivate() {
			manager.EnumerateHandlers(h => h.OnInput -= OnInput);
		}
		/// <summary>
		/// Called when a new input frame is received.
		/// </summary>
		/// <param name="identifier">The input identifier.</param>
		/// <param name="frame">The new input frame.</param>
		protected void OnInput(InputIdentifier identifier, InputFrame frame) {
			lock (_lock) {
				if (_frames.TryGetValue(identifier, out InputFrame frame0)) {
					_events.Add(new InputEvent {
						Identifier = identifier,
						From = frame0,
						To = frame,
					});
					if (frame.IsNull) _frames.Remove(identifier);
					else _frames[identifier] = frame;
				}
				else {
					_events.Add(new InputEvent {
						Identifier = identifier,
						From = new InputFrame(frame.Time),
						To = frame,
					});
					_frames.Add(identifier, frame);
				}
			}
		}
		/// <summary>
		/// Enumerates all the input events in the queue, passes each of them into the given callback function, and then flushes the queue.
		/// </summary>
		/// <param name="cb">The callback function.</param>
		public void EnumerateEvents(Action<InputEvent> cb) {
			if (cb is null) throw new ArgumentNullException(nameof(cb));

			lock (_lock) {
				foreach (var ev in _events) cb(ev);
				_events.Clear();
			}
		}
		/// <summary>
		/// Enumerates all the active identifiers and passes each of them into the given callback function with its current frame.
		/// </summary>
		/// <param name="cb">The callback function.</param>
		public void EnumerateActiveIdentifiers(Action<InputIdentifier, InputFrame> cb) {
			if (cb is null) throw new ArgumentNullException(nameof(cb));

			lock (_lock) {
				foreach (var vec in _frames) cb(vec.Key, vec.Value);
			}
		}
	}
}
