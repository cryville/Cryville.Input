namespace Cryville.Input {
	/// <summary>
	/// Input identifier.
	/// </summary>
	public record struct InputIdentifier {
		/// <summary>
		/// The input source.
		/// </summary>
		public InputSource Source { get; set; }
		/// <summary>
		/// The input ID.
		/// </summary>
		/// <remarks>
		/// <para>This property is used to distinguish different inputs on the input source. For example, a touch screen that supports simultaneous touches may assign a unique ID to each finger.</para>
		/// </remarks>
		public int Id { get; set; }
		/// <inheritdoc />
		public override readonly string ToString() => $"{Source},{Id}";
	}
}
