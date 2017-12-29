using System;
using xnaMugen.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Drawing
{
	internal class SpriteFile : Resource
	{
		public SpriteFile(SpriteSystem spritesystem, File file, SpriteFileVersion version, List<SpriteFileData> data, bool sharedpalette)
		{
			if (spritesystem == null) throw new ArgumentNullException(nameof(spritesystem));
			if (file == null) throw new ArgumentNullException(nameof(file));
			if (data == null) throw new ArgumentNullException(nameof(data));

			m_spritesystem = spritesystem;
			m_file = file;
			m_version = version;
			m_collection = new SpriteFileDataCollection(data);
			m_cachedsprites = new Dictionary<SpriteId, Sprite>();
			m_sharedpalette = sharedpalette;
		}

		public override string ToString()
		{
			return Filepath;
		}

		private bool TryGetSpriteData(SpriteId id, out SpriteFileData data, out int dataindex)
		{
			data = null;
			dataindex = int.MinValue;

			if (id == SpriteId.Invalid) return false;

			var index = m_collection.GetIndex(id);
			if (index == int.MinValue) return false;

			var sfd = m_collection.GetData(index);
			if (sfd == null) return false;

			data = sfd;
			dataindex = index;
			return true;
		}

		public Sprite GetSprite(SpriteId id)
		{
			if (id == SpriteId.Invalid) return null;

			if (m_cachedsprites.ContainsKey(id)) return m_cachedsprites[id];

			SpriteFileData data;
			int dataindex;
			if (TryGetSpriteData(id, out data, out dataindex) == false) return null;

			Point size;
			Texture2D pixels;
			Texture2D palette;
			var paletteoverride = false;
			var ownpixels = true;
			var ownpalette = true;

			if (data.PcxSize > 0)
			{
				m_file.SeekFromBeginning(data.FileOffset);
				if (m_spritesystem.LoadImage(m_file, data.PcxSize, out size, out pixels, out palette) == false)
				{
					Log.Write(LogLevel.Warning, LogSystem.SpriteSystem, "Cannot load PCX image data from '{0}' for sprite #{1}", Filepath, id);

					data.IsValid = false;
					return null;
				}
			}
			else
			{
				var shareddata = m_collection.GetData(data.SharedIndex);
				if (shareddata == null || shareddata.Id == id) return null;

				var sharedSprite = GetSprite(shareddata.Id);
				if (sharedSprite == null) return null;

				size = sharedSprite.Size;
				paletteoverride = sharedSprite.PaletteOverride;
				pixels = sharedSprite.Pixels;
				palette = sharedSprite.Palette;
				ownpixels = false;
				ownpalette = false;
			}

			if (data.CopyLastPalette && dataindex != 0)
			{
				var previousdata = m_collection.GetData(dataindex - 1);
				if (previousdata != null && previousdata.Id != id)
				{
					var previoussprite = GetSprite(previousdata.Id);
					if (previoussprite != null)
					{
						paletteoverride = previoussprite.PaletteOverride;
						ownpalette = false;

						palette = previoussprite.Palette;
					}
				}
			}

			if (id == new SpriteId(0, 0) || id == SpriteId.SmallPortrait) paletteoverride = true;

			var sprite = new Sprite(size, data.Axis, ownpixels, pixels, ownpalette, palette, paletteoverride);
			m_cachedsprites.Add(id, sprite);

			return sprite;
		}

		public void LoadAllSprites()
		{
			for (var i = 0; i != m_collection.Count; ++i)
			{
				var data = m_collection.GetData(i);
				if (data == null) continue;

				GetSprite(data.Id);
			}
		}

		public Texture2D GetFirstPalette()
		{
			if (m_collection.Count == 0) return null;

			var data = m_collection.GetData(0);
			if (data == null) return null;

			var sprite = GetSprite(data.Id);
			if (sprite == null) return null;

			return sprite.Palette;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_cachedsprites != null)
				{
					foreach (var sprite in m_cachedsprites.Values)
					{
						if (sprite != null) sprite.Dispose();
					}

					m_cachedsprites.Clear();
				}
			}

			base.Dispose(disposing);
		}

		public SpriteSystem SpriteSystem => m_spritesystem;

		public string Filepath => m_file.Filepath;

		public SpriteFileVersion Version => m_version;

		public bool SharedPalette => m_sharedpalette;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteSystem m_spritesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly File m_file;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_sharedpalette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteFileVersion m_version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteFileDataCollection m_collection;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<SpriteId, Sprite> m_cachedsprites;

		#endregion
	}
}