using System;
using System.Collections.Generic;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Drawing
{
	class SpriteFileDataCollection
	{
		public SpriteFileDataCollection(List<SpriteFileData> data)
		{
			if (data == null) throw new ArgumentNullException("data");

			m_indexeddata = data;
			m_indexcache = new Dictionary<SpriteId, Int32>();

			for (Int32 i = 0; i != Count; ++i)
			{
				SpriteFileData sfd = m_indexeddata[i];

				if (sfd == null) continue;
				if (m_indexcache.ContainsKey(sfd.Id) == true) continue;

				m_indexcache.Add(sfd.Id, i);
			}
		}

		void SanityCheck(SpriteFileData data, Int32 index)
		{
			if (data == null) throw new ArgumentNullException("data");

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

			if (index == 0 && data.CopyLastPalette == true)
			{
				Log.Write(LogLevel.Warning, LogSystem.SpriteSystem, "First sprite data it set to copy previous, non-existant palette");
			}

			if (data.SharedIndex < 0 || data.SharedIndex >= Count)
			{
				data.IsValid = false;
				Log.Write(LogLevel.Warning, LogSystem.SpriteSystem, "Sprite data #{0}, id #{1} referencing invalid sprite data index", index, data.Id);
			}
		}

		public SpriteFileData GetData(Int32 index)
		{
			if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException("index");

			SpriteFileData data = m_indexeddata[index];

			if (data.IsValid == null)
			{
				data.IsValid = true;
				SanityCheck(data, index);
			}

			return (data.IsValid == true) ? data : null;
		}

		public Int32 GetIndex(SpriteId id)
		{
			Int32 index;
			if (m_indexcache.TryGetValue(id, out index) == false) return Int32.MinValue;

			return index;
		}

		public Dictionary<SpriteId, Int32>.KeyCollection.Enumerator GetEnumerator()
		{
			return m_indexcache.Keys.GetEnumerator();
		}

		public Int32 Count
		{
			get { return m_indexeddata.Count; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<SpriteFileData> m_indexeddata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<SpriteId, Int32> m_indexcache;

		#endregion
	}
}
