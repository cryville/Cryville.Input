using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace Cryville.Input.Xamarin.Android {
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android sensor input.
	/// </summary>
	public abstract partial class AndroidSensorHandler : InputHandler {
		static SensorManager _manager;

		readonly InternalListener _listener;
		readonly Dictionary<Sensor, int> _sensors = [];
		readonly IList<Sensor> _candidateSensors;

		/// <summary>
		/// Creates an instance of the <see cref="AndroidSensorHandler" /> class.
		/// </summary>
		/// <param name="activity">The activity.</param>
		/// <param name="type">The sensor type.</param>
		/// <param name="dimension">The dimension.</param>
		protected AndroidSensorHandler(Activity activity, SensorType type, byte dimension) {
			ArgumentNullException.ThrowIfNull(activity);

			if (dimension < 0 || dimension > 4) throw new ArgumentOutOfRangeException(nameof(dimension));
			_manager ??= (SensorManager)activity.GetSystemService(Context.SensorService);
			m_typeName = WordBoundaryRegex().Replace(type.ToString(), " ");
			m_dimension = dimension;
			_listener = new InternalListener(this);

			_candidateSensors = _manager.GetSensorList(type);
			if (_candidateSensors.Count == 0) throw new IllegalStateException("Sensor not found");
		}

		[GeneratedRegex(@"(?<=[a-z])(?=[A-Z])")]
		private static partial Regex WordBoundaryRegex();

		/// <inheritdoc />
		protected override void Activate() {
			int workingSensorCount = 0;
			foreach (Sensor sensor in _candidateSensors) {
				if (_manager.RegisterListener(_listener, sensor, SensorDelay.Fastest)) {
					_sensors.Add(sensor, workingSensorCount++);
				}
			}
		}

		/// <inheritdoc />
		protected override void Deactivate() {
			_manager.UnregisterListener(_listener);
			_sensors.Clear();
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing) {
			if (disposing) {
				Deactivate();
				_listener.Dispose();
			}
		}

		/// <inheritdoc />
		public override bool IsNullable => false;

		readonly byte m_dimension;
		/// <inheritdoc />
		public override byte Dimension => m_dimension;

		readonly string m_typeName;
		/// <inheritdoc />
		public override string GetTypeName(int type) => type switch {
			0 => m_typeName,
			_ => throw new ArgumentOutOfRangeException(nameof(type)),
		};

		/// <inheritdoc />
		public override double GetCurrentTimestamp() {
			return SystemClock.ElapsedRealtimeNanos() / 1e9;
		}

		sealed class InternalListener(AndroidSensorHandler handler) : Java.Lang.Object, ISensorEventListener {
			public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy) { }

			public void OnSensorChanged(SensorEvent e) {
				if (!handler._sensors.TryGetValue(e.Sensor, out int id)) return;
				IList<float> v = e.Values;
				double time = e.Timestamp / 1e9;
				switch (handler.m_dimension) {
					case 0: handler.Feed(0, id, new InputFrame(time)); break;
					case 1: handler.Feed(0, id, new InputFrame(time, new InputVector(v[0]))); break;
					case 2: handler.Feed(0, id, new InputFrame(time, new InputVector(v[0], v[1]))); break;
					case 3: handler.Feed(0, id, new InputFrame(time, new InputVector(v[0], v[1], v[2]))); break;
					case 4: handler.Feed(0, id, new InputFrame(time, new InputVector(v[0], v[1], v[2], v[3]))); break;
				}
				handler.Batch(time);
			}
		}
	}

	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android accelerometer sensor input.
	/// </summary>
	public class AndroidAccelerometerHandler(Activity activity) : AndroidSensorHandler(activity, SensorType.Accelerometer, 3) {
		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android accelerometer (uncalibrated) sensor input.
	/// </summary>
	[SupportedOSPlatform("android26.0")]
	public class AndroidAccelerometerUncalibratedHandler(Activity activity) : AndroidSensorHandler(activity, SensorType.AccelerometerUncalibrated, 3) {
		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android game rotation vector sensor input.
	/// </summary>
	public class AndroidGameRotationVectorHandler(Activity activity) : AndroidSensorHandler(activity, SensorType.GameRotationVector, 4) {
		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension(),
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android gravity sensor input.
	/// </summary>
	public class AndroidGravityHandler(Activity activity) : AndroidSensorHandler(activity, SensorType.Gravity, 3) {
		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android gyroscope sensor input.
	/// </summary>
	public class AndroidGyroscopeHandler(Activity activity) : AndroidSensorHandler(activity, SensorType.Gyroscope, 3) {
		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension { Time = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android gyroscope (uncalibrated) sensor input.
	/// </summary>
	public class AndroidGyroscopeUncalibratedHandler(Activity activity) : AndroidSensorHandler(activity, SensorType.GyroscopeUncalibrated, 3) {
		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension { Time = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android linear acceleration sensor input.
	/// </summary>
	public class AndroidLinearAccelerationHandler(Activity activity) : AndroidSensorHandler(activity, SensorType.LinearAcceleration, 3) {
		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android magnetic field sensor input.
	/// </summary>
	public class AndroidMagneticFieldHandler(Activity activity) : AndroidSensorHandler(activity, SensorType.MagneticField, 3) {
		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension { Mass = 1, Time = -2, ElectricCurrent = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android magnetic field (uncalibrated) sensor input.
	/// </summary>
	public class AndroidMagneticFieldUncalibratedHandler(Activity activity) : AndroidSensorHandler(activity, SensorType.MagneticFieldUncalibrated, 3) {
		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension { Mass = 1, Time = -2, ElectricCurrent = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android rotation vector sensor input.
	/// </summary>
	public class AndroidRotationVectorHandler(Activity activity) : AndroidSensorHandler(activity, SensorType.RotationVector, 4) {
		static readonly ReferenceCue _refCue = new() {
			PhysicalDimension = new PhysicalDimension(),
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
}
