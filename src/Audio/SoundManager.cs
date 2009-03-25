using System;
using System.Collections.Generic;
using Microsoft.DirectX.DirectSound;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;
using System.Diagnostics;

namespace xnaMugen.Audio
{
	/// <summary>
	/// Governs playing the sounds contained within a SND file.
	/// </summary>
	class SoundManager : Resource
	{
		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="soundsystem">The SoundSystem that created this SoundManager.</param>
		/// <param name="filepath">The relative filepath to the managed SND file.</param>
		/// <param name="sounds">The sounds contained in the SND file.</param>
		public SoundManager(SoundSystem soundsystem, String filepath, ReadOnlyDictionary<SoundId, SecondaryBuffer> sounds)
		{
			if (soundsystem == null) throw new ArgumentNullException("soundsystem");
			if (filepath == null) throw new ArgumentNullException("filepath");
			if (sounds == null) throw new ArgumentNullException("sounds");

			m_soundsystem = soundsystem;
			m_filepath = filepath;
			m_sounds = sounds;
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_soundsystem != null)
				{
					Stop();
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Creates a new SoundManager capable of the playing the same sounds as this instance.
		/// </summary>
		/// <returns></returns>
		public SoundManager Clone()
		{
			return new SoundManager(m_soundsystem, Filepath, m_sounds);
		}

		/// <summary>
		/// Stops the sound that is currently playing on a given channel number.
		/// </summary>
		/// <param name="channelindex">The channel number to stop.</param>
		public void Stop(Int32 channelindex)
		{
			if (channelindex < 0) throw new ArgumentOutOfRangeException("channelindex");

			Channel channel = GetChannel(channelindex);
			if (channel != null) channel.Stop();
		}

		/// <summary>
		/// Stops all sounds that this instance is currently playing.
		/// </summary>
		public void Stop()
		{
			m_soundsystem.Stop(this);
		}

		/// <summary>
		/// Determines whether the SoundManager contains a sound indentified by a given SoundId.
		/// </summary>
		/// <param name="id">the SoundId that indentifies a sound.</param>
		/// <returns>true is the SoundManager contains the requested sound; false otherwise.</returns>
		public Boolean ContainsSound(SoundId id)
		{
			if (id == SoundId.Invalid) return false;

			return m_sounds.ContainsKey(id);
		}

		/// <summary>
		/// Pans a currently playing sound left or right of it current location, in pixels.
		/// </summary>
		/// <param name="channelindex">The channel where the sound to be panned in playing on.</param>
		/// <param name="panning">The distance from the current panning location, in pixels, that the sound should be panned.</param>
		public void RelativePan(Int32 channelindex, Int32 panning)
		{
			if (channelindex < 0) throw new ArgumentOutOfRangeException("channelindex");

			Channel channel = GetChannel(channelindex);
			if (channel != null && channel.IsPlaying == true) channel.RelativePan(panning);
		}

		/// <summary>
		/// Pans a currently playing sound to a new location, in pixels.
		/// </summary>
		/// <param name="channelindex">The channel where the sound to be panned in playing on.</param>
		/// <param name="panning">The distance from the center of the screen, in pixels, that the sound should be panned.</param>
		public void AbsolutePan(Int32 channelindex, Int32 panning)
		{
			if (channelindex < 0) throw new ArgumentOutOfRangeException("channelindex");

			Channel channel = GetChannel(channelindex);
			if (channel != null && channel.IsPlaying == true) channel.AbsolutePan(panning);
		}

		/// <summary>
		/// Plays the requested sound.
		/// </summary>
		/// <param name="id">The SoundId identifing the sound to be played.</param>
		/// <returns>The Channel where the sound is playing, if available. If the sound cannot be played, then null.</returns>
		public Channel Play(SoundId id)
		{
			return Play(-1, id, false, 0, 1, false);
		}

		/// <summary>
		/// Plays the requested sound.
		/// </summary>
		/// <param name="channelindex">The non-negative channelnumber where the sound is to be played. -1 for any available channel.</param>
		/// <param name="id">The SoundId identifing the sound to be played.</param>
		/// <param name="lowpriority">If true and there is a currently playing sound on the same channel, the requested sound will not play.</param>
		/// <param name="volume">The volume level of the sound.</param>
		/// <param name="freqmul">The multiplier applied the sound to change its pitch. 1.0f for no change.</param>
		/// <param name="looping">Whether the sound should loop automatically until stopped.</param>
		/// <returns>The Channel where the sound is playing, if available. If the sound cannot be played, then null.</returns>
		public Channel Play(Int32 channelindex, SoundId id, Boolean lowpriority, Int32 volume, Single freqmul, Boolean looping)
		{
			if (channelindex < -1) throw new ArgumentOutOfRangeException("channelindex");
			if (id == SoundId.Invalid) return null;

			SecondaryBuffer sound = null;
			if (m_sounds.TryGetValue(id, out sound) == false) return null;

			Channel channel = GetChannel(channelindex);
			if (channel == null || (channel.IsPlaying == true && lowpriority == true)) return null;

			volume += m_soundsystem.GlobalVolume;
			volume = Misc.Clamp(volume, (Int32)Volume.Min, (Int32)Volume.Max);

			channel.Play(new ChannelId(this, channelindex), m_soundsystem.CloneBuffer(sound), freqmul, looping, volume);

			return channel;
		}

		/// <summary>
		/// Returns a requested Channel if available.
		/// </summary>
		/// <param name="index">-1 for any available Channel. And number greater than or equal to zero for a specific channel.</param>
		/// <returns>The requested Channel if available. Null if no Channels are available for sound playback.</returns>
		Channel GetChannel(Int32 index)
		{
			return m_soundsystem.GetChannel(new ChannelId(this, index));
		}

		/// <summary>
		/// Generates a System.String whose value is an representation of this instance.
		/// </summary>
		/// <returns>A System.String representation of this instance.</returns>
		public override String ToString()
		{
			return Filepath;
		}

		/// <summary>
		/// The filepath to the SND file whose sounds this instance manages.
		/// </summary>
		/// <returns>The filepath the the respective SND file.</returns>
		public String Filepath
		{
			get { return m_filepath; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SoundSystem m_soundsystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		readonly ReadOnlyDictionary<SoundId, SecondaryBuffer> m_sounds;

		#endregion
	}
}