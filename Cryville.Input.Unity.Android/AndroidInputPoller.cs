using System.Collections.Generic;
using System.Threading;

namespace Cryville.Input.Unity.Android {
	internal class AndroidInputPoller {
		static AndroidInputPoller m_instance;
		public static AndroidInputPoller Instance {
			get {
				if (m_instance == null) m_instance = new AndroidInputPoller();
				return m_instance;
			}
		}

		readonly Thread _thread;
		private AndroidInputPoller() {
			_thread = new Thread(ThreadLogic);
			_thread.Start();
		}

		readonly Dictionary<int, AndroidInputHandler> _handlers = new Dictionary<int, AndroidInputHandler>();
		public void Register(int id, AndroidInputHandler handler) {
			_handlers[id] = handler;
		}

		void ThreadLogic() {
			while (true) {
				while (NativeMethods.AndroidInputProxy_Poll(out var frame) == 1) {
					_handlers[frame.hid].OnFeed(frame.id, frame.action, frame.time, frame.x, frame.y, frame.z, frame.w);
				}
				Thread.Sleep(1);
			}
		}
	}
}
