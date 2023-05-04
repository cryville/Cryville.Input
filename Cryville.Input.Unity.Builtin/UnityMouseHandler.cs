using System;
using UnityEngine;
using unity = UnityEngine;

namespace Cryville.Input.Unity {
	public class UnityMouseHandler : InputHandler {
		GameObject _receiver;

		public UnityMouseHandler() {
			if (!unity::Input.mousePresent) {
				throw new NotSupportedException("Unity mouse is not supported on this device");
			}
		}

		protected override void Activate() {
			_receiver = new GameObject("__mouseRecv__");
			_receiver.AddComponent<UnityMouseReceiver>().SetHandler(this);
		}

		protected override void Deactivate() {
			if (_receiver) GameObject.Destroy(_receiver);
		}

		public override void Dispose(bool disposing) {
			if (disposing) {
				Deactivate();
			}
		}

		public override bool IsNullable { get { return false; } }

		public override byte Dimension { get { return 2; } }

		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1 },
			RelativeUnit = RelativeUnit.Pixel,
		};
		public override ReferenceCue ReferenceCue => _refCue;

		public override string GetTypeName(int type) {
			switch (type) {
				case 0: return "Mouse Position";
				default: throw new ArgumentOutOfRangeException("type");
			}
		}

		public override double GetCurrentTimestamp() {
			return Time.realtimeSinceStartupAsDouble;
		}

		public class UnityMouseReceiver : MonoBehaviour {
			UnityMouseHandler _handler;
			public void SetHandler(UnityMouseHandler h) {
				_handler = h;
			}
			void Update() {
				double time = Time.realtimeSinceStartupAsDouble;
				Vector3 pos = unity::Input.mousePosition;
				_handler.Feed(0, 0, new InputFrame(time, new InputVector(pos.x, pos.y)));
			}
		}
	}
}
