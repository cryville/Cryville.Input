package world.cryville.input.unity.android;

final class NativeMethods {
	private NativeMethods() { }
	public static native void feed(int hid, int id, int action, long time, float x, float y, float z, float t);
}