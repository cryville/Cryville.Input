using Cryville.Common.Logging;
using Cryville.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cryville.Input {
	/// <summary>
	/// Input manager.
	/// </summary>
	public class InputManager {
		/// <summary>
		/// A map of handler types to lists of parameters to be initialized.
		/// </summary>
		public static readonly Dictionary<Type, object[]> HandlerRegistries = new Dictionary<Type, object[]>();
		readonly HashSet<InputHandler> _handlers = new HashSet<InputHandler>();
		readonly Dictionary<Type, InputHandler> _typeMap = new Dictionary<Type, InputHandler>();
		/// <summary>
		/// Creates an instance of the <see cref="InputManager" /> class and tries to initialize all the handlers with their corresponding parameters in <see cref="HandlerRegistries" />.
		/// </summary>
		public InputManager() {
			foreach (var t in HandlerRegistries) {
				try {
					if (!typeof(InputHandler).IsAssignableFrom(t.Key)) continue;
					var h = (InputHandler)Activator.CreateInstance(t.Key, t.Value);
					_typeMap.Add(t.Key, h);
					_handlers.Add(h);
					Logger.Log("main", 1, "Input", "Initialized {0}", TypeNameHelper.GetSimpleName(t.Key));
				}
				catch (TargetInvocationException ex) {
					if (ex.InnerException is NotSupportedException)
						Logger.Log("main", 3, "Input", "Attempted to initialize {0}: {1}", TypeNameHelper.GetSimpleName(t.Key), ex.InnerException?.Message);
					else
						Logger.Log("main", 3, "Input", "An error occurred while initializing {0}: {1}", TypeNameHelper.GetSimpleName(t.Key), ex.InnerException);
				}
			}
		}
		/// <summary>
		/// Gets the handler with the specified type name.
		/// </summary>
		/// <param name="typeName">The type name.</param>
		/// <returns>The handler with the specified type name. <see langword="null" /> if not found or not initialized.</returns>
		public InputHandler GetHandlerByTypeName(string typeName) {
			var type = Type.GetType(typeName);
			if (type == null) return null;
			_typeMap.TryGetValue(type, out InputHandler result);
			return result;
		}
		/// <summary>
		/// Enumerates all initialized handlers and passes each of them into a callback function.
		/// </summary>
		/// <param name="cb">The callback function.</param>
		public void EnumerateHandlers(Action<InputHandler> cb) {
			foreach (var h in _handlers) cb(h);
		}
	}
}
