using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryville.Input.Unity {
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Unity GUI input.
	/// </summary>
	/// <typeparam name="T">The GUI event receiver type.</typeparam>
	public class UnityGuiInputHandler<T> : InputHandler where T : UnityGuiEventReceiver {
		readonly GameObject _receiver;
		readonly T _recvComp;

		/// <summary>
		/// Creates an instance of the <see cref="UnityGuiInputHandler{T}" /> class.
		/// </summary>
		public UnityGuiInputHandler() {
			_receiver = new GameObject("__guiRecv__");
			_recvComp = _receiver.AddComponent<T>();
			_recvComp.SetFeedCallback(Feed);
			_recvComp.SetBatchCallback(Batch);
		}

		/// <inheritdoc />
		protected override void Activate() {
			_recvComp.enabled = true;
		}

		/// <inheritdoc />
		protected override void Deactivate() {
			if (_recvComp) _recvComp.enabled = false;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing) {
			if (disposing) {
				Deactivate();
			}
		}

		/// <inheritdoc />
		public override bool IsNullable { get { return true; } }

		/// <inheritdoc />
		public override byte Dimension { get { return 0; } }

		static readonly ReferenceCue _refCue = new ReferenceCue { };
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;

		/// <inheritdoc />
		public override string GetTypeName(int type) {
			return _recvComp.GetKeyName(type);
		}

		/// <inheritdoc />
		public override double GetCurrentTimestamp() {
			return Time.realtimeSinceStartupAsDouble;
		}
	}

	/// <summary>
	/// Unity GUI event receiver.
	/// </summary>
	public abstract class UnityGuiEventReceiver : MonoBehaviour {
		/// <summary>
		/// The callback function to be called when a new input frame is received.
		/// </summary>
		protected Action<int, int, InputFrame> Feed;
		/// <summary>
		/// Sets the callback function to be called when a new input frame is received.
		/// </summary>
		/// <param name="h">The callback function to be called when a new input frame is received.</param>
		public void SetFeedCallback(Action<int, int, InputFrame> h) {
			Feed = h;
		}
		/// <summary>
		/// The callback function to be called when the current input batch is finished receiving.
		/// </summary>
		protected Action<double> Batch;
		/// <summary>
		/// Sets the callback function to be called when the current input batch is finished receiving.
		/// </summary>
		/// <param name="h">The callback function to be called when the current input batch is finished receiving.</param>
		public void SetBatchCallback(Action<double> h) {
			Batch = h;
		}
		/// <summary>
		/// The set of currently active keys.
		/// </summary>
		protected readonly HashSet<int> ActiveKeys = new HashSet<int>();
		/// <summary>
		/// Gets the friendly name of the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>The friendly name of the specified key.</returns>
		public abstract string GetKeyName(int key);
		void Awake() {
			useGUILayout = false;
		}
		void Update() {
			Batch(Time.realtimeSinceStartupAsDouble);
		}
	}

	public class UnityKeyReceiver : UnityGuiEventReceiver {
		/// <inheritdoc />
		public override string GetKeyName(int type) {
			return Enum.GetName(typeof(KeyCode), type);
		}
		void OnGUI() {
			var e = Event.current;
			if (e.keyCode == KeyCode.None) return;
			double time = Time.realtimeSinceStartupAsDouble;
			var key = (int)e.keyCode;
			switch (e.type) {
				case EventType.KeyDown:
					if (!ActiveKeys.Contains(key)) {
						Feed(key, 0, new InputFrame(time, new InputVector()));
						ActiveKeys.Add(key);
					}
					break;
				case EventType.KeyUp:
					ActiveKeys.Remove(key);
					Feed(key, 0, new InputFrame(time));
					break;
			}
		}
	}

	public class UnityMouseReceiver : UnityGuiEventReceiver {
		/// <inheritdoc />
		public override string GetKeyName(int type) {
			switch (type) {
				case 0: return "Mouse Left Button";
				case 1: return "Mouse Right Button";
				case 2: return "Mouse Middle Button";
				default: return string.Format("Mouse Button {0}", type);
			}
		}
		void OnGUI() {
			var e = Event.current;
			double time = Time.realtimeSinceStartupAsDouble;
			var key = e.button;
			switch (e.type) {
				case EventType.MouseDown:
					if (!ActiveKeys.Contains(key)) {
						Feed(key, 0, new InputFrame(time, new InputVector()));
						ActiveKeys.Add(key);
					}
					break;
				case EventType.MouseUp:
					ActiveKeys.Remove(key);
					Feed(key, 0, new InputFrame(time));
					break;
			}
		}
	}
}
