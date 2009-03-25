using System;
using Microsoft.DirectX.DirectSound;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace xnaMugen.Audio
{
	/// <summary>
	/// A location where an instance of a sound may be played.
	/// </summary>
	class Channel : Resource
	{
		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		public Channel()
		{
			m_buffer = null;
			m_panning = 0;
			m_paused = false;
			m_playflags = BufferPlayFlags.Default;
			m_usingid = new ChannelId();
		}

		/// <summary>
		/// Plays a sound.
		/// </summary>
		/// <param name="id">The ChannelId identifing the SoundManger playing the sound and the channel number.</param>
		/// <param name="buffer">A SecondaryBuffer containing the sound to be playing.</param>
		/// <param name="frequencymultiplier">The multiplier applied the sound to change its pitch. 1.0f for no change.</param>
		/// <param name="looping">Whether the sound should loop automatically until stopped.</param>
		/// <param name="volume">The volume level of the sound.</param>
		public void Play(ChannelId id, SecondaryBuffer buffer, Single frequencymultiplier, Boolean looping, Int32 volume)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (volume > (Int32)Volume.Max || volume < (Int32)Volume.Min) throw new ArgumentOutOfRangeException("volume");

			Stop();

			m_usingid = id;
			m_buffer = buffer;

			BufferPlayFlags flags = (looping == true) ? BufferPlayFlags.Looping : BufferPlayFlags.Default;

			m_buffer.Frequency = (Int32)(m_buffer.Frequency * frequencymultiplier);
			m_buffer.SetCurrentPosition(0);
			m_buffer.Pan = (Int32)Pan.Center;
			m_buffer.Volume = volume;

			m_playflags = flags;

			try
			{
				m_buffer.Play(0, flags);
			}
			catch { }
		}

		/// <summary>
		/// Pans a currently playing sound left or right of it current location, in pixels.
		/// </summary>
		/// <param name="panning">The distance from the current panning location, in pixels, that the sound should be panned.</param>
		public void RelativePan(Int32 panning)
		{
			AbsolutePan(m_panning + panning);
		}

		/// <summary>
		/// Pans a currently playing sound to a new location, in pixels.
		/// </summary>
		/// <param name="panning">The distance from the center of the screen, in pixels, that the sound should be panned.</param>
		public void AbsolutePan(Int32 panning)
		{
			if (IsPlaying == false) return;

			Int32 halfx = Mugen.ScreenSize.X / 2;

			m_panning = Misc.Clamp(panning, -halfx, halfx);

			Single pan_percentage = (Single)(m_panning + halfx) / (Single)Mugen.ScreenSize.X;

			Int32 pan_amount = (Int32)MathHelper.Lerp((Single)Pan.Left, (Single)Pan.Right, pan_percentage);

			m_buffer.Pan = pan_amount;
		}

		/// <summary>
		/// Stops the sounds currently playing in this Channel.
		/// </summary>
		public void Stop()
		{
			if (m_buffer != null)
			{
				m_buffer.Stop();
				m_buffer.Dispose();
				m_buffer = null;
			}
		}

		public void Pause()
		{
			if (IsPlaying == true)
			{
				m_buffer.Stop();
				m_paused = true;
			}
		}

		public void UnPause()
		{
			if (m_buffer != null && m_paused == true)
			{
				m_paused = false;
				m_buffer.Play(0, m_playflags);
			}
		}

		/// <summary>
		/// Disposes of resources managed by this class instance.
		/// </summary>
		/// <param name="disposing">Determined whether to dispose of managed resources.</param>
		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_buffer != null)
				{
					m_buffer.Stop();
					m_buffer.Dispose();
					m_buffer = null;
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Get whether there is a sound currenly playing in this Channel.
		/// </summary>
		/// <returns>true is a sound is playing; false otherwise.</returns>
		public Boolean IsPlaying
		{
			get { return (m_buffer != null) ? m_buffer.Status.Playing : false; }
		}

		/// <summary>
		/// A identifier for the SoundManager currently using this channel.
		/// </summary>
		/// <returns>If there is a sound playing in this Channel, then the ChannelId identifing the SoundManager and the channel number. Otherwise, a default constructed ChannelId.</returns>
		public ChannelId UsingId
		{
			get { return (IsPlaying == true) ? m_usingid : new ChannelId(); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		SecondaryBuffer m_buffer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_panning;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ChannelId m_usingid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_paused;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		BufferPlayFlags m_playflags;

		#endregion
	}
}