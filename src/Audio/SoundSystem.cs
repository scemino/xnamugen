using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace xnaMugen.Audio
{
	/// <summary>
	/// Controls all sound and audio effects for xnaMugen engine.
	/// </summary>
	class SoundSystem : SubSystem
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="subsystems">Collection of subsystems.</param>
		public SoundSystem(SubSystems subsystems)
			: base(subsystems)
		{
            m_soundcache = new Dictionary<String, ReadOnlyDictionary<SoundId, byte[]>>(StringComparer.OrdinalIgnoreCase);
            m_sounddevice = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo);
			m_volume = 0;
			m_channels = new List<Channel>();

			CreateSoundChannels(10);
		}

		public override void Initialize()
		{
			InitializationSettings settings = GetSubSystem<InitializationSettings>();

			CreateSoundChannels(settings.SoundChannels);
		}

		private void CreateSoundChannels(int count)
		{
			if (count <= 0) throw new ArgumentOutOfRangeException("count");

			if (m_channels.Count != count)
			{
				foreach (Channel channel in m_channels) channel.Dispose();
				m_channels.Clear();

				for (var i = 0; i != count; ++i) m_channels.Add(new Channel());
			}
		}

		/// <summary>
		/// Creates a new SoundManager from a SND file.
		/// </summary>
		/// <param name="filepath">Relative filepath of the SND file.</param>
		/// <returns>A new SoundManager instance capable of playing the sounds contained in a SND file.</returns>
		public SoundManager CreateManager(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			ReadOnlyDictionary<SoundId, byte[]> sounds;
			if (m_soundcache.TryGetValue(filepath, out sounds) == true) return new SoundManager(this, filepath, sounds);

			try
			{
				return new SoundManager(this, filepath, GetSounds(filepath));
			}
			catch (System.IO.FileNotFoundException)
			{
                return new SoundManager(this, filepath, new ReadOnlyDictionary<SoundId, byte[]>(new Dictionary<SoundId, byte[]>()));
			}
		}

		/// <summary>
		/// Creates a duplicate of a sound buffer.
		/// </summary>
		/// <param name="buffer">The buffer to be cloned.</param>
		/// <returns>A duplicate instance of supplied sound buffer.</returns>
		public byte[] CloneBuffer(byte[] buffer)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");

            var newbuffer = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, newbuffer, 0, buffer.Length);
			//newbuffer.Volume = 0;

			return newbuffer;
		}

		/// <summary>
		/// Stops all currently playing sound channels.
		/// </summary>
		public void StopAllSounds()
		{
			foreach (Channel channel in m_channels)
			{
				channel.Stop();
			}
		}

		public void PauseSounds()
		{
			foreach (Channel channel in m_channels)
			{
				channel.Pause();
			}
		}

		public void UnPauseSounds()
		{
			foreach (Channel channel in m_channels)
			{
				channel.UnPause();
			}
		}

		/// <summary>
		/// Stops all sounds that a given SoundManager is currently playing.
		/// </summary>
		/// <param name="manager">The SoundManager to silence.</param>
		public void Stop(SoundManager manager)
		{
			if (manager == null) throw new ArgumentNullException("manager");

			foreach (Channel channel in m_channels)
			{
				if (channel.UsingId.Manager == manager) channel.Stop();
			}
		}

		/// <summary>
		/// Retrieves a speficied Channel for playing a sound.
		/// </summary>
		/// <param name="id">Identifier for the requested Channel.</param>
		/// <returns>The Channel current playing with the supplied identifier. If no Channel is found, than any available Channel is returned. If there are no available channels, then null.</returns>
		public Channel GetChannel(ChannelId id)
		{
			if (id == new ChannelId()) throw new ArgumentException("id");

			if (id.Number == -1)
			{
				foreach (Channel channel in m_channels)
				{
					if (channel.IsPlaying == false) return channel;
				}
			}
			else
			{
				foreach (Channel channel in m_channels)
				{
					if (channel.UsingId == id) return channel;
				}

				foreach (Channel channel in m_channels)
				{
					if (channel.IsPlaying == false) return channel;
				}
			}

			return null;
		}

		/// <summary>
		/// Loads sounds contained in a SND file.
		/// </summary>
		/// <param name="filepath">The filepath of the SND file to load sounds from.</param>
		/// <returns>A cached ReadOnlyDictionary mapping SoundIds to their respective SecondaryBuffers.</returns>
		private ReadOnlyDictionary<SoundId, byte[]> GetSounds(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			var sounds = new Dictionary<SoundId, byte[]>();

			using (IO.File file = GetSubSystem<IO.FileSystem>().OpenFile(filepath))
			{
				var header = new IO.FileHeaders.SoundFileHeader(file);

				file.SeekFromBeginning(header.SubheaderOffset);

				for (var i = 0; i != header.NumberOfSounds; ++i)
				{
					var subheader = new IO.FileHeaders.SoundFileSubHeader(file);

					if (sounds.ContainsKey(subheader.Id) == true)
					{
						Log.Write(LogLevel.Warning, LogSystem.SoundSystem, "Duplicate sound with ID {0} discarded.", subheader.Id);
					}
					else
					{
						try
						{
							sounds.Add(subheader.Id, CreateSoundBuffer(file, subheader.SoundLength));
						}
						catch
						{
							Log.Write(LogLevel.Warning, LogSystem.SoundSystem, "Error reading sound with ID {0}.", subheader.Id);
						}
					}

					file.SeekFromBeginning(subheader.NextOffset);
				}
			}

			ReadOnlyDictionary<SoundId, byte[]> savedsounds = new ReadOnlyDictionary<SoundId, byte[]>(sounds);
			m_soundcache.Add(filepath, savedsounds);

			return savedsounds;
		}

		/// <summary>
		/// Read a part of a SND file and created a new byte[] from the contained sound.
		/// </summary>
		/// <param name="file">The file where the sound to be loaded from. The read position must be set at the beginning the sound part of the file.</param>
		/// <param name="length">The length of the sound in the filestream.</param>
		/// <returns></returns>
        private byte[] CreateSoundBuffer(IO.File file, int length)
		{
			if (file == null) throw new ArgumentNullException("file");
			if (length <= 0) throw new ArgumentOutOfRangeException("length", "length must be greater than zero.");

			//BufferDescription description = new BufferDescription();
			//description.CanGetCurrentPosition = true;
			//description.Control3D = false;
			//description.ControlFrequency = true;
			//description.ControlPan = true;
			//description.ControlPositionNotify = true;
			//description.ControlVolume = true;

            var buffer = file.ReadBytes(length);
			return buffer;
		}

		/// <summary>
		/// Disposes of resources managed by this class instance.
		/// </summary>
		/// <param name="disposing">Determined whether to dispose of managed resources.</param>
		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_channels != null)
				{
					foreach (Channel channel in m_channels)
					{
						channel.Dispose();
					}
				}

				if (m_soundcache != null)
				{
					foreach (var data in m_soundcache.Values)
					{
						foreach (byte[] buffer in data.Values)
						{
							//buffer.Dispose();
						}
					}
				}

				if (m_sounddevice != null)
				{
					m_sounddevice.Dispose();
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Controls volume level for all sounds.
		/// </summary>
		/// <returns>The current volume level.</returns>
		public int GlobalVolume
		{
			get { return m_volume; }

			set
			{
                //if (value > (int)Volume.Max || value < (int)Volume.Min) throw new ArgumentOutOfRangeException("value");

				m_volume = value;
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly DynamicSoundEffectInstance m_sounddevice;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Dictionary<String, ReadOnlyDictionary<SoundId, byte[]>> m_soundcache;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Channel> m_channels;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int m_volume;

		#endregion
	}
}