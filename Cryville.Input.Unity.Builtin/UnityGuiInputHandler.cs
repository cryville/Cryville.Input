using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryville.Input.Unity {
	public class UnityGuiInputHandler<T> : InputHandler where T : UnityGuiEventReceiver {
		GameObject _receiver;
		T _recvComp;

		public UnityGuiInputHandler() { }

		protected override void Activate() {
			_receiver = new GameObject("__guiRecv__");
			_recvComp = _receiver.AddComponent<T>();
			_recvComp.SetCallback(Feed);
		}

		protected override void Deactivate() {
			if (_receiver) GameObject.Destroy(_receiver);
		}

		public override void Dispose(bool disposing) {
			if (disposing) {
				Deactivate();
			}
		}
		
		public override bool IsNullable { get { return true; } }

		public override byte Dimension { get { return 0; } }

		readonly static ReferenceCue _refCue = new ReferenceCue { };
		public override ReferenceCue ReferenceCue => _refCue;

		public override string GetTypeName(int type) {
			return _recvComp.GetKeyName(type);
		}

		public override double GetCurrentTimestamp() {
			return Time.realtimeSinceStartupAsDouble;
		}
	}

	public abstract class UnityGuiEventReceiver : MonoBehaviour {
		protected Action<int, int, InputFrame> Callback;
		protected readonly HashSet<int> Keys = new HashSet<int>();
		public void SetCallback(Action<int, int, InputFrame> h) {
			Callback = h;
		}
		public abstract string GetKeyName(int type);
		void Awake() {
			useGUILayout = false;
		}
		void Update() {
			double time = Time.realtimeSinceStartupAsDouble;
			foreach (var k in Keys) {
				Callback(k, 0, new InputFrame(time, new InputVector()));
			}
		}
	}

	public class UnityKeyReceiver : UnityGuiEventReceiver {
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
					if (!Keys.Contains(key)) {
						Callback(key, 0, new InputFrame(time, new InputVector()));
						Keys.Add(key);
					}
					break;
				case EventType.KeyUp:
					Keys.Remove(key);
					Callback(key, 0, new InputFrame(time));
					break;
			}
		}
	}

	public class UnityMouseReceiver : UnityGuiEventReceiver {
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
					if (!Keys.Contains(key)) {
						Callback(key, 0, new InputFrame(time, new InputVector()));
						Keys.Add(key);
					}
					break;
				case EventType.MouseUp:
					Keys.Remove(key);
					Callback(key, 0, new InputFrame(time));
					break;
			}
		}
	}
}
