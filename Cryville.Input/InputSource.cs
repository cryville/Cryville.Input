using Cryville.Common.Reflection;

namespace Cryville.Input {
	/// <summary>
	/// Input source.
	/// </summary>
	public record struct InputSource {
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
		public override readonly string ToString() => $"{TypeNameHelper.GetSimpleName(Handler.GetType())}:{Handler.GetTypeName(Type)}";
	}
}
