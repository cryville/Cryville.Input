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
		/// A set of handler types to be initialized.
		/// </summary>
		public static readonly HashSet<Type> HandlerRegistries = new HashSet<Type> { };
		readonly HashSet<InputHandler> _handlers = new HashSet<InputHandler>();
		readonly Dictionary<Type, InputHandler> _typeMap = new Dictionary<Type, InputHandler>();
		/// <summary>
		/// Creates an instance of the <see cref="InputManager" /> class and tries to initialize all the handlers in <see cref="HandlerRegistries" />.
		/// </summary>
		public InputManager() {
			foreach (var t in HandlerRegistries) {
				try {
					if (!typeof(InputHandler).IsAssignableFrom(t)) continue;
					var h = (InputHandler)Activator.CreateInstance(t);
					_typeMap.Add(t, h);
					_handlers.Add(h);
					Logger.Log("main", 1, "Input", "Initialized {0}", TypeNameHelper.GetSimpleName(t));
				}
				catch (TargetInvocationException ex) {
					Logger.Log("main", 1, "Input", "Cannot initialize {0}: {1}", TypeNameHelper.GetSimpleName(t), ex.InnerException.Message);
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
