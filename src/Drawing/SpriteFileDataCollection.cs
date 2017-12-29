using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Drawing
{
	internal class SpriteFileDataCollection
	{
		public SpriteFileDataCollection(List<SpriteFileData> data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			m_indexeddata = data;
			m_indexcache = new Dictionary<SpriteId, int>();

			for (var i = 0; i != Count; ++i)
			{
				var sfd = m_indexeddata[i];

				if (sfd == null) continue;
				if (m_indexcache.ContainsKey(sfd.Id)) continue;

				m_indexcache.Add(sfd.Id, i);
			}
		}

		private void SanityCheck(SpriteFileData data, int index)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			if (data.Id.Group < 0 || data.Id.Image < 0)
			{
				data.IsValid = false;
				Log.Write(LogLevel.Warning, LogSystem.SpriteSystem, "Sprite data #{0} has invalid SpriteId #{1}", index, data.Id);
			}

			if (data.PcxSize < 0)
			{
				data.IsValid = false;
				Log.Write(LogLevel.Warning, LogSystem.SpriteSystem, "Sprite data #{0}, id #{1} has invalid image size - {2}", index, data.Id, data.PcxSize);
			}

			if (index == 0 && data.CopyLastPalette)
			{
				Log.Write(LogLevel.Warning, LogSystem.SpriteSystem, "First sprite data it set to copy previous, non-existant palette");
			}

			if (data.SharedIndex < 0 || data.SharedIndex >= Count)
			{
				data.IsValid = false;
				Log.Write(LogLevel.Warning, LogSystem.SpriteSystem, "Sprite data #{0}, id #{1} referencing invalid sprite data index", index, data.Id);
			}
		}

		public SpriteFileData GetData(int index)
		{
			if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));

			var data = m_indexeddata[index];

			if (data.IsValid == null)
			{
				data.IsValid = true;
				SanityCheck(data, index);
			}

			return data.IsValid == true ? data : null;
		}

		public int GetIndex(SpriteId id)
		{
			int index;
			if (m_indexcache.TryGetValue(id, out index) == false) return int.MinValue;

			return index;
		}

		public int Count => m_indexeddata.Count;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<SpriteFileData> m_indexeddata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<SpriteId, int> m_indexcache;

		#endregion
	}
}
