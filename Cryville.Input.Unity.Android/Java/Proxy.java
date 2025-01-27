package world.cryville.input.unity.android;

import android.app.Activity;
import com.unity3d.player.UnityPlayer;
import world.cryville.input.unity.android.NativeMethods;

public abstract class Proxy {
	protected static Activity activity;
	
	static int _count;
	int _id;

	public Proxy() {
		if (activity == null) activity = UnityPlayer.currentActivity;
		_id = _count++;
	}

	public int getId() { return _id; }
	
	public abstract void activate();
	public abstract void deactivate();
	
	protected void feed(int id, int action, long time)                                     { NativeMethods.feed(_id, id, action, time, 0, 0, 0, 0); }
	protected void feed(int id, int action, long time, float x)                            { NativeMethods.feed(_id, id, action, time, x, 0, 0, 0); }
	protected void feed(int id, int action, long time, float x, float y)                   { NativeMethods.feed(_id, id, action, time, x, y, 0, 0); }
	protected void feed(int id, int action, long time, float x, float y, float z)          { NativeMethods.feed(_id, id, action, time, x, y, z, 0); }
	protected void feed(int id, int action, long time, float x, float y, float z, float w) { NativeMethods.feed(_id, id, action, time, x, y, z, w); }
}