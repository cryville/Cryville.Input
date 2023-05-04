using System;

namespace Cryville.Input {
	/// <summary>
	/// Provides cues about the frame of reference.
	/// </summary>
	public class ReferenceCue {
		/// <summary>
		/// The physical dimension.
		/// </summary>
		public PhysicalDimension PhysicalDimension { get; set; }
		/// <summary>
		/// The additional relative unit.
		/// </summary>
		public RelativeUnit RelativeUnit { get; set; }
		/// <summary>
		/// The reference flags.
		/// </summary>
		public ReferenceFlag Flags { get; set; }
		/// <summary>
		/// The origin.
		/// </summary>
		public InputVector Origin { get; set; }
		/// <summary>
		/// The pivot.
		/// </summary>
		public InputVector Pivot { get; set; }
		/// <summary>
		/// Transforms a frame into the reference by applying the offset specified by <see cref="Origin" />.
		/// </summary>
		/// <param name="frame">The input frame.</param>
		/// <returns>The transformed input frame.</returns>
		public InputFrame Transform(InputFrame frame) {
			if (frame.IsNull) return frame;
			var vector = frame.Vector;
			vector -= Origin;
			frame.Vector = vector;
			return frame;
		}
		/// <summary>
		/// Transforms a frame out of the reference by removing the offset specified by <see cref="Origin" />.
		/// </summary>
		/// <param name="frame">The input frame.</param>
		/// <returns>The transformed input frame.</returns>
		public InputFrame InverseTransform(InputFrame frame) {
			if (frame.IsNull) return frame;
			var vector = frame.Vector;
			vector += Origin;
			frame.Vector = vector;
			return frame;
		}
		/// <summary>
		/// Transforms a frame into the reference by applying the offset specified by <see cref="Origin" /> and <see cref="Pivot" />.
		/// </summary>
		/// <param name="frame">The input frame.</param>
		/// <param name="universe">The universe size.</param>
		/// <returns>The transformed input frame.</returns>
		public InputFrame Transform(InputFrame frame, InputVector universe) {
			if (frame.IsNull) return frame;
			var vector = frame.Vector;
			vector = new InputVector(
				Transform(vector.X, (Flags & ReferenceFlag.FlipX) != 0, Pivot.X, universe.X),
				Transform(vector.Y, (Flags & ReferenceFlag.FlipY) != 0, Pivot.Y, universe.Y),
				Transform(vector.Z, (Flags & ReferenceFlag.FlipZ) != 0, Pivot.Z, universe.Z),
				Transform(vector.W, (Flags & ReferenceFlag.FlipW) != 0, Pivot.W, universe.W)
			);
			vector -= Origin;
			frame.Vector = vector;
			return frame;
		}
		/// <summary>
		/// Transforms a frame out of the reference by removing the offset specified by <see cref="Origin" /> and <see cref="Pivot" />.
		/// </summary>
		/// <param name="frame">The input frame.</param>
		/// <param name="universe">The universe size.</param>
		/// <returns>The transformed input frame.</returns>
		public InputFrame InverseTransform(InputFrame frame, InputVector universe) {
			if (frame.IsNull) return frame;
			var vector = frame.Vector;
			vector += Origin;
			vector = new InputVector(
				InverseTransform(vector.X, (Flags & ReferenceFlag.FlipX) != 0, Pivot.X, universe.X),
				InverseTransform(vector.Y, (Flags & ReferenceFlag.FlipY) != 0, Pivot.Y, universe.Y),
				InverseTransform(vector.Z, (Flags & ReferenceFlag.FlipZ) != 0, Pivot.Z, universe.Z),
				InverseTransform(vector.W, (Flags & ReferenceFlag.FlipW) != 0, Pivot.W, universe.W)
			);
			frame.Vector = vector;
			return frame;
		}
		static float Transform(float num, bool flip, float pivot, float universe) {
			if (universe == 0) return num;
			num /= universe;
			if (flip) num = -num;
			num += pivot;
			num *= universe;
			return num;
		}
		static float InverseTransform(float num, bool flip, float pivot, float universe) {
			if (universe == 0) return num;
			num /= universe;
			num -= pivot;
			if (flip) num = -num;
			num *= universe;
			return num;
		}
	}
	/// <summary>
	/// Physical dimension.
	/// </summary>
	public class PhysicalDimension {
		/// <summary>
		/// The dimensions of time.
		/// </summary>
		public int Time { get; set; }
		/// <summary>
		/// The dimensions of length.
		/// </summary>
		public int Length { get; set; }
		/// <summary>
		/// The dimensions of mass.
		/// </summary>
		public int Mass { get; set; }
		/// <summary>
		/// The dimensions of electric current.
		/// </summary>
		public int ElectricCurrent { get; set; }
		/// <summary>
		/// The dimensions of thermodynamic temperature.
		/// </summary>
		public int ThermodynamicTemperature { get; set; }
		/// <summary>
		/// The dimensions of amount of substance.
		/// </summary>
		public int AmountOfSubstance { get; set; }
		/// <summary>
		/// The dimensions of luminous intensity.
		/// </summary>
		public int LuminousIntensity { get; set; }
	}
	/// <summary>
	/// Relative unit.
	/// </summary>
	public enum RelativeUnit {
		/// <summary>
		/// None.
		/// </summary>
		None,
		/// <summary>
		/// Pixel.
		/// </summary>
		Pixel,
	}
	/// <summary>
	/// Reference flag.
	/// </summary>
	[Flags]
	public enum ReferenceFlag {
		/// <summary>
		/// None.
		/// </summary>
		None  = 0,
		/// <summary>
		/// The X axis is flipped.
		/// </summary>
		FlipX = 0b1,
		/// <summary>
		/// The Y axis is flipped.
		/// </summary>
		FlipY = 0b10,
		/// <summary>
		/// The Z axis is flipped.
		/// </summary>
		FlipZ = 0b100,
		/// <summary>
		/// The W axis is flipped.
		/// </summary>
		FlipW = 0b1000,
	}
}
