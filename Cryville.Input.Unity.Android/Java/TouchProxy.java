package world.cryville.input.unity.android;

import android.view.MotionEvent;
import android.view.View;
import com.unity3d.player.UnityPlayer;
import java.lang.reflect.Field;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import world.cryville.input.unity.android.Proxy;

public final class TouchProxy extends Proxy implements View.OnTouchListener {
	public TouchProxy() throws NoSuchFieldException, NoSuchMethodException, IllegalAccessException, InvocationTargetException, SecurityException {
		Field playerField = activity.getClass().getDeclaredField("mUnityPlayer");
		playerField.setAccessible(true);
		UnityPlayer player = (UnityPlayer)playerField.get(activity);
		if (View.class.isInstance(player)) {
			// Before Unity 6
			((View)(Object)player).setOnTouchListener(this);
		}
		else {
			// Unity 6 or later
			Method getSurfaceViewMethod = player.getClass().getMethod("getSurfaceView");
			View view = (View)getSurfaceViewMethod.invoke(player);
			view.setOnTouchListener(this);
		}
	}

	boolean activated;

	@Override
	public void activate() {
		activated = true;
	}

	@Override
	public void deactivate() {
		activated = false;
	}

	@Override
	public boolean onTouch(View v, MotionEvent event) {
		if (!activated) return false;
		int pointerCount = event.getPointerCount();
		int action = event.getActionMasked();
		int actionIndex = event.getActionIndex();
		long time = event.getEventTime();
		for (int i = 0; i < pointerCount; i++) {
			int id = event.getPointerId(i);
			float x = event.getX(i);
			float y = event.getY(i);
			if (action == 5 || action == 6) {
				feed(id, i == actionIndex ? action : -1, time, x, y);
			}
			else {
				feed(id, action, time, x, y);
			}
		}
		feed(0, -2, time);
		return false;
	}
}