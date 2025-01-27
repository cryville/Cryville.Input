using Cryville.Interop.Mono;
using System;
using System.Text.RegularExpressions;

namespace Cryville.Input.Unity.Android {
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android sensor input.
	/// </summary>
	public abstract class AndroidSensorHandler<TSelf> : AndroidInputHandler<TSelf> where TSelf : AndroidSensorHandler<TSelf> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidSensorHandler" /> class.
		/// </summary>
		/// <param name="typeName">The name of the Java class nested in <c>world/cryville/input/unity/android/SensorProxy</c> that performs the low-level jobs.</param>
		/// <param name="dimension">The dimension.</param>
		public AndroidSensorHandler(string typeName, byte dimension) : base("world/cryville/input/unity/android/SensorProxy$" + typeName) {
			m_typeName = Regex.Replace(typeName, @"(?<=[a-z])(?=[A-Z])", " ");
			m_dimension = dimension;
		}

		/// <inheritdoc />
		public override bool IsNullable => false;

		readonly byte m_dimension;
		/// <inheritdoc />
		public override byte Dimension => m_dimension;

		readonly string m_typeName;
		/// <inheritdoc />
		public override string GetTypeName(int type) {
			switch (type) {
				case 0: return m_typeName;
				default: throw new ArgumentOutOfRangeException("type");
			}
		}

		/// <inheritdoc />
		public override double GetCurrentTimestamp() {
			return JavaStaticMethods.SystemClock_elapsedRealtimeNanos() / 1e9;
		}

		private protected sealed override AndroidInputProxy_Callback Callback { get { return OnFeed; } }

		[MonoPInvokeCallback(typeof(AndroidInputProxy_Callback))]
		static void OnFeed(int id, int action, long time, float x, float y, float z, float w) {
			try {
				double timeSecs = time / 1e9;
				Instance.Feed(0, id, new InputFrame(timeSecs, new InputVector(x, y, z, w)));
				Instance.Batch(timeSecs);
			}
			catch (Exception ex) {
				Shared.Logger.Log(4, "Input", "An error occurred while handling an Android sensor event: {0}", ex);
			}
		}
	}

	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android accelerometer sensor input.
	/// </summary>
	public class AndroidAccelerometerHandler : AndroidSensorHandler<AndroidAccelerometerHandler> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidAccelerometerHandler" /> class.
		/// </summary>
		public AndroidAccelerometerHandler() : base("Accelerometer", 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android accelerometer (uncalibrated) sensor input.
	/// </summary>
	public class AndroidAccelerometerUncalibratedHandler : AndroidSensorHandler<AndroidAccelerometerUncalibratedHandler> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidAccelerometerUncalibratedHandler" /> class.
		/// </summary>
		public AndroidAccelerometerUncalibratedHandler() : base("AccelerometerUncalibrated", 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android game rotation vector sensor input.
	/// </summary>
	public class AndroidGameRotationVectorHandler : AndroidSensorHandler<AndroidGameRotationVectorHandler> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidGameRotationVectorHandler" /> class.
		/// </summary>
		public AndroidGameRotationVectorHandler() : base("GameRotationVector", 4) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension(),
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android gravity sensor input.
	/// </summary>
	public class AndroidGravityHandler : AndroidSensorHandler<AndroidGravityHandler> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidGravityHandler" /> class.
		/// </summary>
		public AndroidGravityHandler() : base("Gravity", 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android gyroscope sensor input.
	/// </summary>
	public class AndroidGyroscopeHandler : AndroidSensorHandler<AndroidGyroscopeHandler> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidGyroscopeHandler" /> class.
		/// </summary>
		public AndroidGyroscopeHandler() : base("Gyroscope", 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Time = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android gyroscope (uncalibrated) sensor input.
	/// </summary>
	public class AndroidGyroscopeUncalibratedHandler : AndroidSensorHandler<AndroidGyroscopeUncalibratedHandler> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidGyroscopeUncalibratedHandler" /> class.
		/// </summary>
		public AndroidGyroscopeUncalibratedHandler() : base("GyroscopeUncalibrated", 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Time = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android linear acceleration sensor input.
	/// </summary>
	public class AndroidLinearAccelerationHandler : AndroidSensorHandler<AndroidLinearAccelerationHandler> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidLinearAccelerationHandler" /> class.
		/// </summary>
		public AndroidLinearAccelerationHandler() : base("LinearAcceleration", 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android magnetic field sensor input.
	/// </summary>
	public class AndroidMagneticFieldHandler : AndroidSensorHandler<AndroidMagneticFieldHandler> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidMagneticFieldHandler" /> class.
		/// </summary>
		public AndroidMagneticFieldHandler() : base("MagneticField", 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Mass = 1, Time = -2, ElectricCurrent = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android magnetic field (uncalibrated) sensor input.
	/// </summary>
	public class AndroidMagneticFieldUncalibratedHandler : AndroidSensorHandler<AndroidMagneticFieldUncalibratedHandler> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidMagneticFieldUncalibratedHandler" /> class.
		/// </summary>
		public AndroidMagneticFieldUncalibratedHandler() : base("MagneticFieldUncalibrated", 3) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Mass = 1, Time = -2, ElectricCurrent = -1 },
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
	/// <summary>
	/// An <see cref="InputHandler" /> that handles Android rotation vector sensor input.
	/// </summary>
	public class AndroidRotationVectorHandler : AndroidSensorHandler<AndroidRotationVectorHandler> {
		/// <summary>
		/// Creates an instance of the <see cref="AndroidRotationVectorHandler" /> class.
		/// </summary>
		public AndroidRotationVectorHandler() : base("RotationVector", 4) { }
		static readonly ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension(),
		};
		/// <inheritdoc />
		public override ReferenceCue ReferenceCue => _refCue;
	}
}
