namespace Cryville.Input {
	/// <summary>
	/// Input event.
	/// </summary>
	public struct InputEvent {
		/// <summary>
		/// The identifier.
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
		public override string ToString() {
			return string.Format("[{0}] {1} -> {2}", Identifier, From, To);
		}
	}
}
