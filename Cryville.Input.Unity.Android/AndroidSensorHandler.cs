using Cryville.Common.Interop;
using Cryville.Common.Logging;
using System;
using System.Text.RegularExpressions;

namespace Cryville.Input.Unity.Android {
	public abstract class AndroidSensorHandler<TSelf> : AndroidInputHandler<AndroidSensorHandler<TSelf>> where TSelf : AndroidSensorHandler<TSelf> {
		public AndroidSensorHandler(string typeName, byte dimension) : base("world/cryville/input/unity/android/SensorProxy$" + typeName) {
			m_typeName = Regex.Replace(typeName, @"(?<=[a-z])(?=[A-Z])", " ");
			m_dimension = dimension;
		}

		public override bool IsNullable => false;

		readonly byte m_dimension;
		public override byte Dimension => m_dimension;

		readonly string m_typeName;
		public override string GetTypeName(int type) {
			switch (type) {
				case 0: return m_typeName;
				default: throw new ArgumentOutOfRangeException("type");
			}
		}

		public override double GetCurrentTimestamp() {
			return JavaStaticMethods.SystemClock_elapsedRealtimeNanos() / 1e9;
		}

		private protected sealed override AndroidInputProxy_Callback Callback { get { return OnFeed; } }

		[MonoPInvokeCallback]
		static void OnFeed(int id, int action, long time, float x, float y, float z, float w) {
			try {
				double timeSecs = time / 1e9;
				Instance.Feed(0, id, new InputFrame(timeSecs, new InputVector(x, y, z, w)));
			}
			catch (Exception ex) {
				Logger.Log("main", 4, "Input", "An error occurred while handling an Android sensor event: {0}", ex);
			}
		}
	}

	public class AndroidAccelerometerHandler : AndroidSensorHandler<AndroidAccelerometerHandler> {
		public AndroidAccelerometerHandler() : base("Accelerometer", 3) { }
		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		public override ReferenceCue ReferenceCue => _refCue;
	}
	public class AndroidAccelerometerUncalibratedHandler : AndroidSensorHandler<AndroidAccelerometerUncalibratedHandler> {
		public AndroidAccelerometerUncalibratedHandler() : base("AccelerometerUncalibrated", 3) { }
		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		public override ReferenceCue ReferenceCue => _refCue;
	}
	public class AndroidGameRotationVectorHandler : AndroidSensorHandler<AndroidGameRotationVectorHandler> {
		public AndroidGameRotationVectorHandler() : base("GameRotationVector", 4) { }
		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension(),
		};
		public override ReferenceCue ReferenceCue => _refCue;
	}
	public class AndroidGravityHandler : AndroidSensorHandler<AndroidGravityHandler> {
		public AndroidGravityHandler() : base("Gravity", 3) { }
		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		public override ReferenceCue ReferenceCue => _refCue;
	}
	public class AndroidGyroscopeHandler : AndroidSensorHandler<AndroidGyroscopeHandler> {
		public AndroidGyroscopeHandler() : base("Gyroscope", 3) { }
		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Time = -1 },
		};
		public override ReferenceCue ReferenceCue => _refCue;
	}
	public class AndroidLinearAccelerationHandler : AndroidSensorHandler<AndroidLinearAccelerationHandler> {
		public AndroidLinearAccelerationHandler() : base("LinearAcceleration", 3) { }
		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Length = 1, Time = -2 },
		};
		public override ReferenceCue ReferenceCue => _refCue;
	}
	public class AndroidMagneticFieldHandler : AndroidSensorHandler<AndroidMagneticFieldHandler> {
		public AndroidMagneticFieldHandler() : base("MagneticField", 3) { }
		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Mass = 1, Time = -2, ElectricCurrent = -1 },
		};
		public override ReferenceCue ReferenceCue => _refCue;
	}
	public class AndroidMagneticFieldUncalibratedHandler : AndroidSensorHandler<AndroidMagneticFieldUncalibratedHandler> {
		public AndroidMagneticFieldUncalibratedHandler() : base("MagneticFieldUncalibrated", 3) { }
		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension { Mass = 1, Time = -2, ElectricCurrent = -1 },
		};
		public override ReferenceCue ReferenceCue => _refCue;
	}
	public class AndroidRotationVectorHandler : AndroidSensorHandler<AndroidRotationVectorHandler> {
		public AndroidRotationVectorHandler() : base("RotationVector", 4) { }
		readonly static ReferenceCue _refCue = new ReferenceCue {
			PhysicalDimension = new PhysicalDimension(),
		};
		public override ReferenceCue ReferenceCue => _refCue;
	}
}
