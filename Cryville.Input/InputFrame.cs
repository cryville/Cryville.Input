namespace Cryville.Input {
	/// <summary>
	/// Input frame.
	/// </summary>
	public struct InputFrame {
		/// <summary>
		/// The timestamp in seconds.
		/// </summary>
		public double Time { get; set; }
		/// <summary>
		/// Whether the vector is null.
		/// </summary>
		/// <remarks>
		/// <para>An input frame with this property set to <see langword="true" /> marks the end of life of an input ID (see <see cref="InputIdentifier.Id" />.) This usually occurs when, for example, the button of the device is released.</para>
		/// <para>When this property is set to <see langword="true" />, all the components of the vector is meaningless and should be set to 0.</para>
		/// </remarks>
		public bool IsNull { get; set; }
		/// <summary>
		/// The input vector.
		/// </summary>
		public InputVector Vector { get; set; }
		/// <summary>
		/// Creates an instance of the <see cref="InputFrame" /> struct with <see cref="IsNull" /> set to <see langword="true" />.
		/// </summary>
		/// <param name="time">The timestamp in seconds.</param>
		public InputFrame(double time) {
			Time = time;
			IsNull = true;
			Vector = new InputVector();
		}
		/// <summary>
		/// Creates an instance of the <see cref="InputFrame" /> struct.
		/// </summary>
		/// <param name="time">The timestamp in seconds.</param>
		/// <param name="vector">The input vector.</param>
		public InputFrame(double time, InputVector vector) {
			Time = time;
			IsNull = false;
			Vector = vector;
		}
		/// <inheritdoc />
		public override string ToString() {
			if (IsNull) return string.Format("null@{0}", Time);
			else return string.Format("{0}@{1}", Vector, Time);
		}
	}
}
