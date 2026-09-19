package world.cryville.input.unity.android;

import android.view.ActionMode;
import android.view.KeyEvent;
import android.view.KeyboardShortcutGroup;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.SearchEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.view.accessibility.AccessibilityEvent;
import java.util.List;
import world.cryville.input.unity.android.Proxy;

/**
 * A {@link Proxy} that captures Android touch input by wrapping the activity's
 * {@link Window.Callback}.
 *
 * <p>
 * This used to attach a {@link View.OnTouchListener} to the Unity view (or, on Unity 6, to
 * the frame layout returned by {@code UnityPlayer.getFrameLayout()}). Unity 6 launches with
 * {@code GameActivity} by default, and GameActivity installs its own touch listener on the
 * game surface ({@code GameActivity$3}) which consumes the events before they can reach a
 * listener attached to any parent view. The decor view calls
 * {@link Window.Callback#dispatchTouchEvent} before dispatching an event to the view tree,
 * and the original callback is always invoked unchanged, so this works under both the
 * Activity and GameActivity entry points without disturbing Unity's own input handling.
 * </p>
 */
public final class TouchProxy extends Proxy {
	/**
	 * Creates an instance of the {@link TouchProxy} class.
	 */
	public TouchProxy() {
		if (activity != null) {
			final TouchProxy self = this;
			activity.runOnUiThread(new Runnable() {
				@Override
				public void run() {
					install(self);
				}
			});
		}
	}

	private static void install(TouchProxy proxy) {
		Window window = activity.getWindow();
		if (window == null) return;
		Window.Callback current = window.getCallback();
		TouchProxyWindowCallback callback;
		if (current instanceof TouchProxyWindowCallback) {
			callback = (TouchProxyWindowCallback) current;
		}
		else {
			callback = new TouchProxyWindowCallback(current);
			window.setCallback(callback);
		}
		callback.setProxy(proxy);
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

	void handleTouch(MotionEvent event) {
		if (!activated) return;
		int pointerCount = event.getPointerCount();
		int action = event.getActionMasked();
		int actionIndex = event.getActionIndex();

		int historySize = event.getHistorySize();
		for (int h = 0; h < historySize; h++) {
			long htime = event.getHistoricalEventTime(h);
			for (int i = 0; i < pointerCount; i++) {
				int id = event.getPointerId(i);
				float x = event.getHistoricalX(i, h);
				float y = event.getHistoricalY(i, h);
				if (action == 5 || action == 6) {
					feed(id, i == actionIndex ? action : -1, htime, x, y);
				}
				else {
					feed(id, action, htime, x, y);
				}
			}
		}
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
	}

	/**
	 * Forwards every {@link Window.Callback} call to the original callback, and additionally
	 * reports touch events to the active {@link TouchProxy} before the view tree sees them.
	 */
	private static final class TouchProxyWindowCallback implements Window.Callback {
		private final Window.Callback original;
		private volatile TouchProxy proxy;

		TouchProxyWindowCallback(Window.Callback original) {
			this.original = original;
		}

		void setProxy(TouchProxy proxy) {
			this.proxy = proxy;
		}

		@Override
		public boolean dispatchTouchEvent(MotionEvent event) {
			TouchProxy p = proxy;
			if (p != null) p.handleTouch(event);
			return original != null && original.dispatchTouchEvent(event);
		}

		@Override
		public boolean dispatchGenericMotionEvent(MotionEvent event) {
			return original != null && original.dispatchGenericMotionEvent(event);
		}

		@Override
		public boolean dispatchKeyEvent(KeyEvent event) {
			return original != null && original.dispatchKeyEvent(event);
		}

		@Override
		public boolean dispatchKeyShortcutEvent(KeyEvent event) {
			return original != null && original.dispatchKeyShortcutEvent(event);
		}

		@Override
		public boolean dispatchTrackballEvent(MotionEvent event) {
			return original != null && original.dispatchTrackballEvent(event);
		}

		@Override
		public boolean dispatchPopulateAccessibilityEvent(AccessibilityEvent event) {
			return original != null && original.dispatchPopulateAccessibilityEvent(event);
		}

		@Override
		public View onCreatePanelView(int featureId) {
			return original != null ? original.onCreatePanelView(featureId) : null;
		}

		@Override
		public boolean onCreatePanelMenu(int featureId, Menu menu) {
			return original != null && original.onCreatePanelMenu(featureId, menu);
		}

		@Override
		public boolean onPreparePanel(int featureId, View view, Menu menu) {
			return original != null && original.onPreparePanel(featureId, view, menu);
		}

		@Override
		public boolean onMenuOpened(int featureId, Menu menu) {
			return original != null && original.onMenuOpened(featureId, menu);
		}

		@Override
		public boolean onMenuItemSelected(int featureId, MenuItem item) {
			return original != null && original.onMenuItemSelected(featureId, item);
		}

		@Override
		public void onWindowAttributesChanged(WindowManager.LayoutParams attrs) {
			if (original != null) original.onWindowAttributesChanged(attrs);
		}

		@Override
		public void onContentChanged() {
			if (original != null) original.onContentChanged();
		}

		@Override
		public void onWindowFocusChanged(boolean hasFocus) {
			if (original != null) original.onWindowFocusChanged(hasFocus);
		}

		@Override
		public void onAttachedToWindow() {
			if (original != null) original.onAttachedToWindow();
		}

		@Override
		public void onDetachedFromWindow() {
			if (original != null) original.onDetachedFromWindow();
		}

		@Override
		public void onPanelClosed(int featureId, Menu menu) {
			if (original != null) original.onPanelClosed(featureId, menu);
		}

		@Override
		public boolean onSearchRequested() {
			return original != null && original.onSearchRequested();
		}

		@Override
		public boolean onSearchRequested(SearchEvent searchEvent) {
			return original != null && original.onSearchRequested(searchEvent);
		}

		@Override
		public ActionMode onWindowStartingActionMode(ActionMode.Callback callback) {
			return original != null ? original.onWindowStartingActionMode(callback) : null;
		}

		@Override
		public ActionMode onWindowStartingActionMode(ActionMode.Callback callback, int type) {
			return original != null ? original.onWindowStartingActionMode(callback, type) : null;
		}

		@Override
		public void onActionModeStarted(ActionMode mode) {
			if (original != null) original.onActionModeStarted(mode);
		}

		@Override
		public void onActionModeFinished(ActionMode mode) {
			if (original != null) original.onActionModeFinished(mode);
		}

		@Override
		public void onProvideKeyboardShortcuts(List<KeyboardShortcutGroup> data, Menu menu, int deviceId) {
			if (original != null) original.onProvideKeyboardShortcuts(data, menu, deviceId);
		}

		@Override
		public void onPointerCaptureChanged(boolean hasCapture) {
			if (original != null) original.onPointerCaptureChanged(hasCapture);
		}
	}
}
