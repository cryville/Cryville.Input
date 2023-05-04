using System;

namespace Cryville.Input {
	/// <summary>
	/// Input identifier.
	/// </summary>
	public struct InputIdentifier : IEquatable<InputIdentifier> {
		/// <summary>
		/// The input source.
		/// </summary>
		public InputSource Source { get; set; }
		/// <summary>
		/// The input ID.
		/// </summary>
		/// <remarks>
		/// <para>This property is used to distinguish different inputs on the input source. For example, a touch screen that supports simultaneous touches may assign unique IDs to each finger.</para>
		/// </remarks>
		public int Id { get; set; }
		/// <inheritdoc />
		public override bool Equals(object obj) {
			if (obj == null || !(obj is InputIdentifier)) return false;
			return Equals((InputIdentifier)obj);
		}
		/// <inheritdoc />
		public bool Equals(InputIdentifier other) {
			return Source == other.Source && Id == other.Id;
		}
		/// <inheritdoc />
		public override int GetHashCode() {
			return Source.GetHashCode() ^ ((Id << 16) | (Id >> 16));
		}
		/// <inheritdoc />
		public override string ToString() {
			return string.Format("{0},{1}", Source, Id);
		}
		/// <inheritdoc />
		public static bool operator ==(InputIdentifier lhs, InputIdentifier rhs) {
			return lhs.Equals(rhs);
		}
		/// <inheritdoc />
		public static bool operator !=(InputIdentifier lhs, InputIdentifier rhs) {
			return !lhs.Equals(rhs);
		}
	}
}
