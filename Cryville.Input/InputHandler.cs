using System;

namespace Cryville.Input {
	/// <summary>
	/// Represents the method that will handle the <see cref="InputHandler.OnInput" /> event.
	/// </summary>
	/// <param name="identifier">The input identifier of <paramref name="frame" />.</param>
	/// <param name="frame">The new input frame.</param>
	public delegate void InputFrameHandler(InputIdentifier identifier, InputFrame frame);
	/// <summary>
	/// Represents the method that will handle the <see cref="InputHandler.OnBatch" /> event.
	/// </summary>
	/// <param name="handler">The input handler.</param>
	/// <param name="time">The timestamp of the batch in seconds.</param>
	public delegate void InputBatchHandler(InputHandler handler, double time);
	/// <summary>
	/// Input handler.
	/// </summary>
	public abstract class InputHandler : IDisposable {
		InputFrameHandler m_onInput;
		/// <summary>
		/// Occurs when a new input frame is sent.
		/// </summary>
		/// <remarks>
		/// <para>There is no guarantee that frames are sent in the time order.</para>
		/// </remarks>
		public event InputFrameHandler OnInput {
			add {
				if (m_onInput == null) Activate();
				m_onInput -= value;
				m_onInput += value;
			}
			remove {
				if (m_onInput == null) return;
				m_onInput -= value;
				if (m_onInput == null) Deactivate();
			}
		}

		InputBatchHandler m_onBatch;
		/// <summary>
		/// Occurs when an input batch is finished sending.
		/// </summary>
		/// <remarks>
		/// <para>This event occurs when the device can guarantee that all the active identifiers have sent all their frames before the given timestamp. There may be input frames with later timestamps sent before this event occurs. If an input identifier does not appears between two subsequent <see cref="OnBatch" /> events, then there is no update on it.</para>
		/// <para>However, there is no guarantee that this event will occur when no input identifier is active.</para>
		/// </remarks>
		public event InputBatchHandler OnBatch {
			add {
				m_onBatch -= value;
				m_onBatch += value;
			}
			remove {
				if (m_onBatch == null) return;
				m_onBatch -= value;
			}
		}

		/// <inheritdoc />
		~InputHandler() {
			Dispose(false);
		}
		/// <inheritdoc />
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Activates the input handler and starts receiving new input frames.
		/// </summary>
		protected abstract void Activate();
		/// <summary>
		/// Deactivates the input handler and stops receiving new input frames.
		/// </summary>
		protected abstract void Deactivate();
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">Whether disposing or finalizing.</param>
		public abstract void Dispose(bool disposing);
		/// <summary>
		/// Whether null input frames may be sent by the input handler.
		/// </summary>
		/// <remarks>
		/// <para>See <see cref="InputFrame.IsNull" /> for more information.</para>
		/// </remarks>
		public abstract bool IsNullable { get; }
		/// <summary>
		/// The dimension of the vectors sent by the input handler.
		/// </summary>
		/// <remarks>
		/// <para>Dimension must be an integer from 0 (inclusive) to 4 (inclusive.) The components of the <see cref="InputVector" /> whose indices are beyond the dimension should be set to 0.</para>
		/// </remarks>
		public abstract byte Dimension { get; }
		/// <summary>
		/// The reference cue for the vectors sent by the input handler.
		/// </summary>
		public abstract ReferenceCue ReferenceCue { get; }
		/// <summary>
		/// Gets the friendly name of the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The friendly name of the specified type.</returns>
		/// <remarks>
		/// <para>See <see cref="InputSource.Type"/> for more information.</para>
		/// </remarks>
		public abstract string GetTypeName(int type);
		/// <summary>
		/// Gets the current timestamp of the input handler.
		/// </summary>
		/// <returns>The current timestamp of the input handler in seconds.</returns>
		/// <remarks>
		/// <para>The timestamp returned by this method is obtained from the same source as the <see cref="InputFrame.Time" /> of the frame the handler sends whenever possible. However, there is no guarantee that, when calling this method, all the frames whose <see cref="InputFrame.Time" /> is less than the timestamp returned by this method have been sent.</para>
		/// </remarks>
		public abstract double GetCurrentTimestamp();
		/// <summary>
		/// Sends a new input frame.
		/// </summary>
		/// <param name="type">The type of the input frame.</param>
		/// <param name="id">The ID of the input frame.</param>
		/// <param name="frame">The input frame.</param>
		protected void Feed(int type, int id, InputFrame frame) {
			m_onInput?.Invoke(new InputIdentifier { Source = new InputSource { Handler = this, Type = type }, Id = id }, frame);
		}
		/// <summary>
		/// Marks the end of an input batch.
		/// </summary>
		/// <param name="time">The timestamp of the input batch in seconds.</param>
		/// <remarks>
		/// <para>See <see cref="OnBatch" /> for more information.</para>
		/// </remarks>
		protected void Batch(double time) {
			m_onBatch?.Invoke(this, time);
		}
	}
}
