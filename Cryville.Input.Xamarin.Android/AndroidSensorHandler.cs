using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cryville.Input.Xamarin.Android {
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android sensor input.
	/// </summary>
	public abstract class AndroidSensorHandler : InputHandler {
		static SensorManager _manager;

		readonly InternalListener _listener;
		readonly Dictionary<Sensor, int> _sensors = new Dictionary<Sensor, int>();
		readonly IList<Sensor> _candidateSensors;

		/// <summary>
		/// Creates an instance of the <see cref="AndroidSensorHandler" /> class.
		/// </summary>
		/// <param name="activity">The activity.</param>
		/// <param name="type">The sensor type.</param>
		/// <param name="dimension">The dimension.</param>
		public AndroidSensorHandler(Activity activity, SensorType type, byte dimension) {
			if (dimension < 0 || dimension > 4) throw new ArgumentOutOfRangeException(nameof(dimension));
			_manager ??= (SensorManager)activity.GetSystemService(Context.SensorService);
			m_typeName = Regex.Replace(type.ToString(), @"(?<=[a-z])(?=[A-Z])", " ");
			m_dimension = dimension;
			_listener = new InternalListener(this);

			_candidateSensors = _manager.GetSensorList(type);
			if (_candidateSensors.Count == 0) throw new IllegalStateException("Sensor not found");
		}

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
		public override void Dispose(bool disposing) {
			if (disposing) {
				Deactivate();
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

		class InternalListener : Java.Lang.Object, ISensorEventListener {
			readonly AndroidSensorHandler _handler;

			public InternalListener(AndroidSensorHandler handler) {
				_handler = handler;
			}

			public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy) { }

			public void OnSensorChanged(SensorEvent e) {
				if (!_handler._sensors.TryGetValue(e.Sensor, out int id)) return;
				IList<float> v = e.Values;
				double time = e.Timestamp / 1e9;
				switch (_handler.m_dimension) {
					case 0: _handler.Feed(0, id, new InputFrame(time)); break;
					case 1: _handler.Feed(0, id, new InputFrame(time, new InputVector(v[0]))); break;
					case 2: _handler.Feed(0, id, new InputFrame(time, new InputVector(v[0], v[1]))); break;
					case 3: _handler.Feed(0, id, new InputFrame(time, new InputVector(v[0], v[1], v[2]))); break;
					case 4: _handler.Feed(0, id, new InputFrame(time, new InputVector(v[0], v[1], v[2], v[3]))); break;
				}
				_handler.Batch(time);
			}
		}
	}

	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android accelerometer sensor input.
	/// </summary>
	public class AndroidAccelerometerHandler : AndroidSensorHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidAccelerometerHandler" /> class.
		/// </summary>
		public AndroidAccelerometerHandler(Activity activity) : base(activity, SensorType.Accelerometer, 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android accelerometer (uncalibrated) sensor input.
	/// </summary>
	public class AndroidAccelerometerUncalibratedHandler : AndroidSensorHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidAccelerometerUncalibratedHandler" /> class.
		/// </summary>
		public AndroidAccelerometerUncalibratedHandler(Activity activity) : base(activity, SensorType.AccelerometerUncalibrated, 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android game rotation vector sensor input.
	/// </summary>
	public class AndroidGameRotationVectorHandler : AndroidSensorHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidGameRotationVectorHandler" /> class.
		/// </summary>
		public AndroidGameRotationVectorHandler(Activity activity) : base(activity, SensorType.GameRotationVector, 4) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension(),
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android gravity sensor input.
	/// </summary>
	public class AndroidGravityHandler : AndroidSensorHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidGravityHandler" /> class.
		/// </summary>
		public AndroidGravityHandler(Activity activity) : base(activity, SensorType.Gravity, 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android gyroscope sensor input.
	/// </summary>
	public class AndroidGyroscopeHandler : AndroidSensorHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidGyroscopeHandler" /> class.
		/// </summary>
		public AndroidGyroscopeHandler(Activity activity) : base(activity, SensorType.Gyroscope, 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Time = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android gyroscope (uncalibrated) sensor input.
	/// </summary>
	public class AndroidGyroscopeUncalibratedHandler : AndroidSensorHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidGyroscopeUncalibratedHandler" /> class.
		/// </summary>
		public AndroidGyroscopeUncalibratedHandler(Activity activity) : base(activity, SensorType.GyroscopeUncalibrated, 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Time = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android linear acceleration sensor input.
	/// </summary>
	public class AndroidLinearAccelerationHandler : AndroidSensorHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidLinearAccelerationHandler" /> class.
		/// </summary>
		public AndroidLinearAccelerationHandler(Activity activity) : base(activity, SensorType.LinearAcceleration, 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android magnetic field sensor input.
	/// </summary>
	public class AndroidMagneticFieldHandler : AndroidSensorHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidMagneticFieldHandler" /> class.
		/// </summary>
		public AndroidMagneticFieldHandler(Activity activity) : base(activity, SensorType.MagneticField, 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Mass = 1, Time = -2, ElectricCurrent = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android magnetic field (uncalibrated) sensor input.
	/// </summary>
	public class AndroidMagneticFieldUncalibratedHandler : AndroidSensorHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidMagneticFieldUncalibratedHandler" /> class.
		/// </summary>
		public AndroidMagneticFieldUncalibratedHandler(Activity activity) : base(activity, SensorType.MagneticFieldUncalibrated, 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Mass = 1, Time = -2, ElectricCurrent = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android rotation vector sensor input.
	/// </summary>
	public class AndroidRotationVectorHandler : AndroidSensorHandler {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidRotationVectorHandler" /> class.
		/// </summary>
		public AndroidRotationVectorHandler(Activity activity) : base(activity, SensorType.RotationVector, 4) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension(),
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
}
