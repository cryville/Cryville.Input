using System.Runtime.InteropServices;

namespace Cryville.Input.Unity.Android {
	struct ProxiedInputFrame {
		public int hid;
		public int id;
		public int action;
		public long time;
		public float x;
		public float y;
		public float z;
		public float w;
	};
	internal static class NativeMethods {
		[DllImport("AndroidInputProxy")]
		[PreserveSig]
		public static extern int AndroidInputProxy_Poll(out ProxiedInputFrame frame);
	}
}
