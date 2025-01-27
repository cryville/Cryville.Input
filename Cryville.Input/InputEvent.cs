namespace Cryville.Input {
	/// <summary>
	/// Input event.
	/// </summary>
	public record struct InputEvent {
		/// <summary>
		/// The input identifier.
		/// </summary>
		public InputIdentifier Identifier { get; set; }
		/// <summary>
		/// The input frame last received.
		/// </summary>
		public InputFrame From { get; set; }
		/// <summary>
		/// The new input frame received.
		/// </summary>
		public InputFrame To { get; set; }
		/// <inheritdoc />
		public override readonly string ToString() => $"[{Identifier}] {From} -> {To}";
	}
}
