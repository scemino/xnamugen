using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Audio;

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
			m_panning = 0;
			m_paused = false;
			m_usingid = new ChannelId();
		}

		/// <summary>
		/// Plays a sound.
		/// </summary>
		/// <param name="id">The ChannelId identifing the SoundManger playing the sound and the channel number.</param>
		/// <param name="buffer">A byte[] containing the sound to be playing.</param>
		/// <param name="frequencymultiplier">The multiplier applied the sound to change its pitch. 1.0f for no change.</param>
		/// <param name="looping">Whether the sound should loop automatically until stopped.</param>
		/// <param name="volume">The volume level of the sound.</param>
		public void Play(ChannelId id, byte[] buffer, float frequencymultiplier, bool looping, int volume)
		{
			if (buffer == null) throw new ArgumentNullException(nameof(buffer));

			Stop();

			m_usingid = id;

            try
			{
                using (var ms = new MemoryStream(buffer))
                {
                    m_soundEffect = SoundEffect.FromStream(ms).CreateInstance();
                }
                // TODO: pitch, volume, pan
				//m_soundEffect.Pitch = frequencymultiplier;
                //m_soundEffect.Volume = volume;
                //m_soundEffect.Pan = m_panning;
				m_soundEffect.IsLooped = looping;
				m_soundEffect.Play();
			}
			catch { }
		}

		/// <summary>
		/// Pans a currently playing sound left or right of it current location, in pixels.
		/// </summary>
		/// <param name="panning">The distance from the current panning location, in pixels, that the sound should be panned.</param>
		public void RelativePan(int panning)
		{
			AbsolutePan(m_panning + panning);
		}

		/// <summary>
		/// Pans a currently playing sound to a new location, in pixels.
		/// </summary>
		/// <param name="panning">The distance from the center of the screen, in pixels, that the sound should be panned.</param>
		public void AbsolutePan(int panning)
		{
			if (IsPlaying == false) return;

			int halfx = Mugen.ScreenSize.X / 2;

			m_panning = Misc.Clamp(panning, -halfx, halfx);

            float pan_percentage = (float)(m_panning + halfx) / (float)Mugen.ScreenSize.X;

            // TODO: panning 
            //int pan_amount = (Int32)MathHelper.Lerp((float)Pan.Left, (float)Pan.Right, pan_percentage);

			//m_buffer.Pan = pan_amount;
		}

		/// <summary>
		/// Stops the sounds currently playing in this Channel.
		/// </summary>
		public void Stop()
		{
            if (m_soundEffect != null)
			{
                m_soundEffect.Stop();
                m_soundEffect.Dispose();
                m_soundEffect = null;
			}
		}

		public void Pause()
		{
			if (IsPlaying == true)
			{
                m_soundEffect.Stop();
				m_paused = true;
			}
		}

		public void UnPause()
		{
            if (m_soundEffect != null && m_paused == true)
			{
				m_paused = false;
                m_soundEffect.Play();
			}
		}

		/// <summary>
		/// Disposes of resources managed by this class instance.
		/// </summary>
		/// <param name="disposing">Determined whether to dispose of managed resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing == true)
			{
                if (m_soundEffect != null)
				{
                    m_soundEffect.Stop();
                    m_soundEffect.Dispose();
                    m_soundEffect = null;
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Get whether there is a sound currenly playing in this Channel.
		/// </summary>
		/// <returns>true is a sound is playing; false otherwise.</returns>
		public bool IsPlaying
		{
            get => (m_soundEffect != null) && m_soundEffect.State == SoundState.Playing;
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
        private int m_panning;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ChannelId m_usingid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool m_paused;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SoundEffectInstance m_soundEffect;

        #endregion
    }
}