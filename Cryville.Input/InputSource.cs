using Cryville.Common.Reflection;
using System;

namespace Cryville.Input {
	/// <summary>
	/// Input source.
	/// </summary>
	public struct InputSource : IEquatable<InputSource> {
		/// <summary>
		/// The input handler.
		/// </summary>
		public InputHandler Handler { get; set; }
		/// <summary>
		/// The type of the input source as an identifier of a component of the input handler.
		/// </summary>
		/// <remarks>
		/// <para>This property is used to distinguish different components of the input handler. For example, each key on the keyboard is assigned a unique type number. Use <see cref="InputHandler.GetTypeName(int)" /> to get a friendly name of a specific type.</para>
		/// </remarks>
		public int Type { get; set; }
		/// <inheritdoc />
		public override bool Equals(object obj) {
			if (obj == null || !(obj is InputSource)) return false;
			return Equals((InputSource)obj);
		}
		/// <inheritdoc />
		public bool Equals(InputSource other) {
			return Handler == other.Handler && Type == other.Type;
		}
		/// <inheritdoc />
		public override int GetHashCode() {
			return Handler.GetHashCode() ^ Type;
		}
		/// <inheritdoc />
		public override string ToString() {
			return string.Format("{0}:{1}", TypeNameHelper.GetSimpleName(Handler.GetType()), Handler.GetTypeName(Type));
		}
		/// <inheritdoc />
		public static bool operator ==(InputSource lhs, InputSource rhs) {
			return lhs.Equals(rhs);
		}
		/// <inheritdoc />
		public static bool operator !=(InputSource lhs, InputSource rhs) {
			return !lhs.Equals(rhs);
		}
	}
}
