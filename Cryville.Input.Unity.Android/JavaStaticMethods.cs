using System;
using UnityEngine;

namespace Cryville.Input.Unity.Android {
	internal static class JavaStaticMethods {
		static bool _init;
		static IntPtr _t_SystemClock;
		static IntPtr _m_SystemClock_elapsedRealtimeNanos;
		static IntPtr _m_SystemClock_uptimeMillis;
		static readonly jvalue[] _p_void = new jvalue[0];
		public static void Init() {
			if (_init) return;
			_init = true;
			var _lt_SystemClock = AndroidJNI.FindClass("android/os/SystemClock");
			_t_SystemClock = AndroidJNI.NewGlobalRef(_lt_SystemClock);
			_m_SystemClock_elapsedRealtimeNanos = AndroidJNI.GetStaticMethodID(_lt_SystemClock, "elapsedRealtimeNanos", "()J");
			_m_SystemClock_uptimeMillis = AndroidJNI.GetStaticMethodID(_lt_SystemClock, "uptimeMillis", "()J");
			AndroidJNI.DeleteLocalRef(_lt_SystemClock);
		}
		public static long SystemClock_elapsedRealtimeNanos() {
			return AndroidJNI.CallStaticLongMethod(_t_SystemClock, _m_SystemClock_elapsedRealtimeNanos, _p_void);
		}
		public static long SystemClock_uptimeMillis() {
			return AndroidJNI.CallStaticLongMethod(_t_SystemClock, _m_SystemClock_uptimeMillis, _p_void);
		}
	}
}
