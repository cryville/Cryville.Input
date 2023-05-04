using System;
using UnityEngine;
using unity = UnityEngine;

namespace Cryville.Input.Unity {
	public class UnityTouchHandler : InputHandler {
		GameObject _receiver;

		public UnityTouchHandler() {
			if (!unity::Input.touchSupported) {
				throw new NotSupportedException("Unity touch is not supported on this device");
			}
		}

		protected override void Activate() {
			_receiver = new GameObject("__touchRecv__");
			_receiver.AddComponent<UnityPointerReceiver>().SetHandler(this);
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

		public override byte Dimension { get { return 2; } }

		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1 },
			RelativeUnit = RelativeUnit.Pixel,
		};
		public override ReferenceCue ReferenceCue => _refCue;

		public override string GetTypeName(int type) {
			switch (type) {
				case 0: return "Touch";
				default: throw new ArgumentOutOfRangeException("type");
			}
		}

		public override double GetCurrentTimestamp() {
			return Time.realtimeSinceStartupAsDouble;
		}

		public class UnityPointerReceiver : MonoBehaviour {
			UnityTouchHandler _handler;
			public void SetHandler(UnityTouchHandler h) {
				_handler = h;
			}
			void Update() {
				double time = Time.realtimeSinceStartupAsDouble;
				for (int i = 0; i < unity::Input.touchCount; i++) {
					var t = unity::Input.GetTouch(i);
					Vector2 pos = t.position;
					var vec = new InputFrame(time, new InputVector(pos.x, pos.y));
					_handler.Feed(0, t.fingerId, vec);
					if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) {
						_handler.Feed(0, t.fingerId, new InputFrame(time));
					}
				}
			}
		}
	}
}
