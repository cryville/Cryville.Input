using System.Runtime.InteropServices;

namespace Cryville.Input.Unity.Android {
	internal delegate void AndroidInputProxy_Callback(int id, int action, long time, float x, float y, float z, float w);
	internal static class NativeMethods {
		[DllImport("AndroidInputProxy")]
		public static extern void AndroidInputProxy_RegisterCallback(int hid, AndroidInputProxy_Callback cb);
	}
}
