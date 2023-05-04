using Cryville.Common.Interop;
using Cryville.Common.Logging;
using System;

namespace Cryville.Input.Unity.Android {
	public class AndroidTouchHandler : AndroidInputHandler<AndroidTouchHandler> {
		public AndroidTouchHandler() : base("world/cryville/input/unity/android/TouchProxy") { }

		public override bool IsNullable => true;

		public override byte Dimension => 2;

		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1 },
			RelativeUnit = RelativeUnit.Pixel,
			Flags = ReferenceFlag.FlipY,
			Pivot = new InputVector(0, 1),
		};
		public override ReferenceCue ReferenceCue => _refCue;

		public override string GetTypeName(int type) {
			switch (type) {
				case 0: return "Android Touch";
				default: throw new ArgumentOutOfRangeException("type");
			}
		}

		public override double GetCurrentTimestamp() {
			return JavaStaticMethods.SystemClock_uptimeMillis() / 1000.0;
		}

		private protected override AndroidInputProxy_Callback Callback { get { return OnFeed; } }

		[MonoPInvokeCallback]
		static void OnFeed(int id, int action, long time, float x, float y, float z, float w) {
			try {
				double timeSecs = time / 1000.0;
				Instance.Feed(0, id, new InputFrame(timeSecs, new InputVector(x, y)));
				if (action == 1 /*ACTION_UP*/ || action == 3 /*ACTION_CANCEL*/ || action == 6 /*ACTION_POINTER_UP*/)
					Instance.Feed(0, id, new InputFrame(timeSecs));
			}
			catch (Exception ex) {
				Logger.Log("main", 4, "Input", "An error occurred while handling an Android touch event: {0}", ex);
			}
		}
	}
}
