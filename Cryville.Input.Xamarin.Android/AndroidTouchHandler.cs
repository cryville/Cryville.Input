using Android.OS;
using Android.Views;
using System;

namespace Cryville.Input.Xamarin.Android {
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android touch input.
	/// </summary>
	public class AndroidTouchHandler : InputHandler {
		readonly InternalListener _listener;

		/// <summary>
		/// Creates an instance of the <see cref="AndroidTouchHandler" /> class.
		/// </summary>
		public AndroidTouchHandler(View view) {
			ArgumentNullException.ThrowIfNull(view);
			_listener = new InternalListener(view, this);
		}

		/// <inheritdoc />
		protected override void Activate() {
			_listener._activated = true;
		}

		/// <inheritdoc />
		protected override void Deactivate() {
			_listener._activated = false;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing) {
			if (disposing) {
				Deactivate();
				_listener.Dispose();
			}
		}

		/// <inheritdoc />
		public override bool IsNullable => true;

		/// <inheritdoc />
		public override byte Dimension => 2;

		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension { Length = 1 },
			RelativeUnit = RelativeUnit.Pixel,
			Flags = ReferenceFlag.FlipY,
			Pivot = new InputVector(0, 1),
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;

		/// <inheritdoc />
		public override string GetTypeName(int type) => type switch {
			0 => "Android Touch",
			_ => throw new ArgumentOutOfRangeException(nameof(type)),
		};

		/// <inheritdoc />
		public override double GetCurrentTimestamp() {
			return SystemClock.UptimeMillis() / 1000.0;
		}

		sealed class InternalListener : Java.Lang.Object, View.IOnTouchListener {
			readonly AndroidTouchHandler _handler;
			public bool _activated;

			public InternalListener(View view, AndroidTouchHandler handler) {
				view.SetOnTouchListener(this);
				_handler = handler;
			}

			public bool OnTouch(View v, MotionEvent e) {
				if (!_activated) return false;
				int pointerCount = e.PointerCount;
				var action = e.ActionMasked;
				int actionIndex = e.ActionIndex;

				int historySize = e.HistorySize;
				for (int h = 0; h < historySize; h++) {
					long htime = e.GetHistoricalEventTime(h);
					for (int i = 0; i < pointerCount; i++) {
						int id = e.GetPointerId(i);
						float x = e.GetHistoricalX(i, h);
						float y = e.GetHistoricalY(i, h);
						if (action == MotionEventActions.PointerDown || action == MotionEventActions.PointerUp) {
							Feed(i == actionIndex ? action : MotionEventActions.Move, actionIndex, htime, i, id, x, y);
						}
						else {
							Feed(action, actionIndex, htime, i, id, x, y);
						}
					}
				}
				double time = e.EventTime / 1e3;
				for (int i = 0; i < pointerCount; i++) {
					int id = e.GetPointerId(i);
					float x = e.GetX(i);
					float y = e.GetY(i);
					Feed(action, actionIndex, time, i, id, x, y);
				}
				_handler.Batch(time);
				return false;
			}

			void Feed(MotionEventActions action, int actionIndex, double time, int i, int id, float x, float y) {
				_handler.Feed(0, id, new(time, new(x, y)));
				if (
					action == MotionEventActions.Up ||
					action == MotionEventActions.Cancel ||
					(action == MotionEventActions.PointerUp && i == actionIndex)
				) {
					_handler.Feed(0, id, new(time));
				}
			}
		}
	}
}
