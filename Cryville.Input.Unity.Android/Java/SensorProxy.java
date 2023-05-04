package world.cryville.input.unity.android;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import java.lang.IllegalStateException;
import java.lang.IndexOutOfBoundsException;
import java.util.HashMap;
import java.util.List;
import world.cryville.input.unity.android.Proxy;

public abstract class SensorProxy extends Proxy implements SensorEventListener {
	static SensorManager manager;

	int dimension;

	HashMap<Sensor, Integer> sensors;
	List<Sensor> candidateSensors;

	public SensorProxy(int type, int dim) throws IllegalStateException, IndexOutOfBoundsException {
		if (manager == null) manager = (SensorManager)activity.getSystemService(Context.SENSOR_SERVICE);
		dimension = dim;
		if (dimension < 0 || dimension > 4) throw new IndexOutOfBoundsException("Invalid dimension");

		sensors = new HashMap<Sensor, Integer>();
		candidateSensors = manager.getSensorList(type);
		if (candidateSensors.size() == 0) throw new IllegalStateException("Sensor not found");
	}

	@Override
	public void activate() {
		int workingSensorCount = 0;
		for (Sensor sensor : candidateSensors) {
			if (manager.registerListener(this, sensor, SensorManager.SENSOR_DELAY_FASTEST)) {
				sensors.put(sensor, workingSensorCount++);
			}
		}
	}

	@Override
	public void deactivate() {
		manager.unregisterListener(this);
		sensors.clear();
	}
	
	@Override
	public void onAccuracyChanged(Sensor sensor, int accuracy) { }
	
	@Override
	public void onSensorChanged(SensorEvent event) {
		Integer id = sensors.get(event.sensor);
		if (id == null) return;
		float[] v = event.values;
		switch (dimension) {
			case 0: feed(0, id, event.timestamp); break;
			case 1: feed(0, id, event.timestamp, v[0]); break;
			case 2: feed(0, id, event.timestamp, v[0], v[1]); break;
			case 3: feed(0, id, event.timestamp, v[0], v[1], v[2]); break;
			case 4: feed(0, id, event.timestamp, v[0], v[1], v[2], v[3]); break;
		}
	}

	public static class Accelerometer extends SensorProxy {
		public Accelerometer() { super(Sensor.TYPE_ACCELEROMETER, 3); }
	}

	public static class AccelerometerUncalibrated extends SensorProxy {
		public AccelerometerUncalibrated() { super(Sensor.TYPE_ACCELEROMETER_UNCALIBRATED, 3); }
	}
	
	public static class GameRotationVector extends SensorProxy {
		public GameRotationVector() { super(Sensor.TYPE_GAME_ROTATION_VECTOR, 4); }
	}

	public static class Gravity extends SensorProxy {
		public Gravity() { super(Sensor.TYPE_GRAVITY, 3); }
	}

	public static class Gyroscope extends SensorProxy {
		public Gyroscope() { super(Sensor.TYPE_GYROSCOPE, 3); }
	}

	public static class GyroscopeUncalibrated extends SensorProxy {
		public GyroscopeUncalibrated() { super(Sensor.TYPE_GYROSCOPE_UNCALIBRATED, 3); }
	}

	public static class LinearAcceleration extends SensorProxy {
		public LinearAcceleration() { super(Sensor.TYPE_LINEAR_ACCELERATION, 3); }
	}

	public static class MagneticField extends SensorProxy {
		public MagneticField() { super(Sensor.TYPE_MAGNETIC_FIELD, 3); }
	}

	public static class MagneticFieldUncalibrated extends SensorProxy {
		public MagneticFieldUncalibrated() { super(Sensor.TYPE_MAGNETIC_FIELD_UNCALIBRATED, 3); }
	}

	public static class RotationVector extends SensorProxy {
		public RotationVector() { super(Sensor.TYPE_ROTATION_VECTOR, 4); }
	}
}