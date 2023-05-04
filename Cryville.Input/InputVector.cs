namespace Cryville.Input {
	/// <summary>
	/// Input vector.
	/// </summary>
	public struct InputVector {
		/// <summary>
		/// The first component of the vector.
		/// </summary>
		public float X { get; set; }
		/// <summary>
		/// The second component of the vector.
		/// </summary>
		public float Y { get; set; }
		/// <summary>
		/// The third component of the vector.
		/// </summary>
		public float Z { get; set; }
		/// <summary>
		/// The fourth component of the vector.
		/// </summary>
		public float W { get; set; }
		/// <summary>
		/// Creates an instance of the <see cref="InputVector" /> struct of one dimension.
		/// </summary>
		/// <param name="x">The first component of the vector.</param>
		public InputVector(float x) : this(x, 0, 0, 0) { }
		/// <summary>
		/// Creates an instance of the <see cref="InputVector" /> struct of two dimensions.
		/// </summary>
		/// <param name="x">The first component of the vector.</param>
		/// <param name="y">The second component of the vector.</param>
		public InputVector(float x, float y) : this(x, y, 0, 0) { }
		/// <summary>
		/// Creates an instance of the <see cref="InputVector" /> struct of three dimensions.
		/// </summary>
		/// <param name="x">The first component of the vector.</param>
		/// <param name="y">The second component of the vector.</param>
		/// <param name="z">The third component of the vector.</param>
		public InputVector(float x, float y, float z) : this(x, y, z, 0) { }
		/// <summary>
		/// Creates an instance of the <see cref="InputVector" /> struct of four dimensions.
		/// </summary>
		/// <param name="x">The first component of the vector.</param>
		/// <param name="y">The second component of the vector.</param>
		/// <param name="z">The third component of the vector.</param>
		/// <param name="w">The fourth component of the vector.</param>
		public InputVector(float x, float y, float z, float w) {
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
		/// <inheritdoc />
		public static InputVector operator +(InputVector a, InputVector b) {
			return new InputVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
		}
		/// <inheritdoc />
		public static InputVector operator -(InputVector a, InputVector b) {
			return new InputVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
		}
		/// <inheritdoc />
		public static InputVector operator *(float k, InputVector v) {
			return new InputVector(k * v.X, k * v.Y, k * v.Z, k * v.W);
		}
		/// <inheritdoc />
		public static InputVector operator -(InputVector a) {
			return new InputVector(-a.X, -a.Y, -a.Z, -a.W);
		}
		/// <inheritdoc />
		public override string ToString() {
			return string.Format("({0:G5}, {1:G5}, {2:G5}, {3:G5})", X, Y, Z, W);
		}
	}
}
