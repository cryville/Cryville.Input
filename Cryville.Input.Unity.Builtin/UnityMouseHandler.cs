using System;
using UnityEngine;
using unity = UnityEngine;

namespace Cryville.Input.Unity {
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Unity mouse input.
	/// </summary>
	public class UnityMouseHandler : InputHandler {
		GameObject _receiver;

		/// <summary>
		/// Creates an instance of the <see cref="UnityMouseHandler" /> class.
		/// </summary>
		/// <exception cref="NotSupportedException">Unity mouse is not supported on the current device.</exception>
		public UnityMouseHandler() {
			if (!unity::Input.mousePresent) {
				throw new NotSupportedException("Unity mouse is not supported on this device");
			}
		}

		/// <inheritdoc />
		protected override void Activate() {
			_receiver = new GameObject("__mouseRecv__");
			_receiver.AddComponent<UnityMouseReceiver>().SetHandler(this);
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
		public override bool IsNullable { get { return false; } }

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
				case 0: return "Mouse Position";
				default: throw new ArgumentOutOfRangeException("type");
			}
		}

		/// <inheritdoc />
		public override double GetCurrentTimestamp() {
			return Time.realtimeSinceStartupAsDouble;
		}

		/// <summary>
		/// Unity mouse receiver.
		/// </summary>
		public class UnityMouseReceiver : MonoBehaviour {
			UnityMouseHandler _handler;
			/// <summary>
			/// Sets the <see cref="UnityMouseHandler" />.
			/// </summary>
			/// <param name="h">The <see cref="UnityMouseHandler" />.</param>
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
