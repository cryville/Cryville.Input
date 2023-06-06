using System;
using UnityEngine;
using unity = UnityEngine;

namespace Cryville.Input.Unity {
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Unity touch input.
	/// </summary>
	public class UnityTouchHandler : InputHandler {
		GameObject _receiver;

		/// <summary>
		/// Creates an instance of the <see cref="UnityTouchHandler" /> class.
		/// </summary>
		/// <exception cref="NotSupportedException">Unity touch is not supported on the current device.</exception>
		public UnityTouchHandler() {
#if !UNITY_EDITOR // In the simulator, touch works but `touchSupported` returns false (by 2021.3)
			if (!unity::Input.touchSupported) {
				throw new NotSupportedException("Unity touch is not supported on this device");
			}
#endif
		}

		/// <inheritdoc />
		protected override void Activate() {
			_receiver = new GameObject("__touchRecv__");
			_receiver.AddComponent<UnityTouchReceiver>().SetHandler(this);
		}

		/// <inheritdoc />
		protected override void Deactivate() {
			if (_receiver) GameObject.Destroy(_receiver);
		}

		/// <inheritdoc />
		public override void Dispose(bool disposing) {
			if (disposing) {
				Deactivate();
			}
		}

		/// <inheritdoc />
		public override bool IsNullable { get { return true; } }

		/// <inheritdoc />
		public override byte Dimension { get { return 2; } }

		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1 },
			RelativeUnit = RelativeUnit.Pixel,
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;

		/// <inheritdoc />
		public override string GetTypeName(int type) {
			switch (type) {
				case 0: return "Touch";
				default: throw new ArgumentOutOfRangeException("type");
			}
		}

		/// <inheritdoc />
		public override double GetCurrentTimestamp() {
			return Time.realtimeSinceStartupAsDouble;
		}

		/// <summary>
		/// Unity touch receiver.
		/// </summary>
		public class UnityTouchReceiver : MonoBehaviour {
			UnityTouchHandler _handler;
			/// <summary>
			/// Sets the <see cref="UnityTouchHandler" />.
			/// </summary>
			/// <param name="h">The <see cref="UnityTouchHandler" />.</param>
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
				_handler.Batch(time);
			}
		}
	}
}
