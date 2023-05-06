using Cryville.Common.Logging;
using System;

namespace Cryville.Input.Unity.Android {
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android touch input.
	/// </summary>
	public class AndroidTouchHandler : AndroidInputHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidTouchHandler" /> class.
		/// </summary>
		public AndroidTouchHandler() : base("world/cryville/input/unity/android/TouchProxy") { }

		/// <inheritdoc />
		public override bool IsNullable => true;

		/// <inheritdoc />
		public override byte Dimension => 2;

		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1 },
			RelativeUnit = RelativeUnit.Pixel,
			Flags = ReferenceFlag.FlipY,
			Pivot = new InputVector(0, 1),
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;

		/// <inheritdoc />
		public override string GetTypeName(int type) {
			switch (type) {
				case 0: return "Android Touch";
				default: throw new ArgumentOutOfRangeException("type");
			}
		}

		/// <inheritdoc />
		public override double GetCurrentTimestamp() {
			return JavaStaticMethods.SystemClock_uptimeMillis() / 1000.0;
		}

		internal override void OnFeed(int id, int action, long time, float x, float y, float z, float w) {
			try {
				double timeSecs = time / 1000.0;
				Feed(0, id, new InputFrame(timeSecs, new InputVector(x, y)));
				if (action == 1 /*ACTION_UP*/ || action == 3 /*ACTION_CANCEL*/ || action == 6 /*ACTION_POINTER_UP*/)
					Feed(0, id, new InputFrame(timeSecs));
			}
			catch (Exception ex) {
				Logger.Log("main", 4, "Input", "An error occurred while handling an Android touch event: {0}", ex);
			}
		}
	}
}
