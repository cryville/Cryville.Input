using System;
using UnityEngine;

namespace Cryville.Input.Unity.Android {
	public abstract class AndroidInputHandler<TSelf> : InputHandler where TSelf : AndroidInputHandler<TSelf> {
		protected static TSelf Instance { get; private set; }

		readonly IntPtr _t_T;
		static readonly jvalue[] _p_void = new jvalue[0];
		readonly IntPtr _i_T;

		readonly IntPtr _m_T_activate;
		readonly IntPtr _m_T_deactivate;

		bool _activated;

		public AndroidInputHandler(string className) {
			if (Instance != null)
				throw new InvalidOperationException("AndroidInputHandler already created");
			if (Environment.OSVersion.Platform != PlatformID.Unix)
				throw new NotSupportedException("Android input is not supported on this device");
			Instance = (TSelf)this;

			JavaStaticMethods.Init();

			var _lt_T = AndroidJNI.FindClass(className);
			_t_T = AndroidJNI.NewGlobalRef(_lt_T);
			AndroidJNI.DeleteLocalRef(_lt_T);

			var _m_T_init = AndroidJNI.GetMethodID(_t_T, "<init>", "()V");
			var _li_T = AndroidJNI.NewObject(_t_T, _m_T_init, _p_void);
			_i_T = AndroidJNI.NewGlobalRef(_li_T);
			AndroidJNI.DeleteLocalRef(_li_T);

			var _m_T_getId = AndroidJNI.GetMethodID(_t_T, "getId", "()I");
			_m_T_activate = AndroidJNI.GetMethodID(_t_T, "activate", "()V");
			_m_T_deactivate = AndroidJNI.GetMethodID(_t_T, "deactivate", "()V");

			NativeMethods.AndroidInputProxy_RegisterCallback(
				AndroidJNI.CallIntMethod(_i_T, _m_T_getId, _p_void),
				Callback
			);
		}

		protected override void Activate() {
			if (_activated) return;
			_activated = true;
			AndroidJNI.CallVoidMethod(_i_T, _m_T_activate, _p_void);
		}

		protected override void Deactivate() {
			if (!_activated) return;
			_activated = false;
			AndroidJNI.CallVoidMethod(_i_T, _m_T_deactivate, _p_void);
		}

		public override void Dispose(bool disposing) {
			if (disposing) {
				Deactivate();
				AndroidJNI.DeleteGlobalRef(_i_T);
				AndroidJNI.DeleteGlobalRef(_t_T);
			}
		}

		private protected abstract AndroidInputProxy_Callback Callback { get; }
	}
}
