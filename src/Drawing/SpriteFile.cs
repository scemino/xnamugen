using System;
using xnaMugen.IO;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;

namespace xnaMugen.Drawing
{
	class SpriteFile : Resource
	{
		public SpriteFile(SpriteSystem spritesystem, File file, SpriteFileVersion version, List<SpriteFileData> data, Boolean sharedpalette)
		{
			if (spritesystem == null) throw new ArgumentNullException("spritesystem");
			if (file == null) throw new ArgumentNullException("file");
			if (data == null) throw new ArgumentNullException("data");

			m_spritesystem = spritesystem;
			m_file = file;
			m_version = version;
			m_collection = new SpriteFileDataCollection(data);
			m_cachedsprites = new Dictionary<SpriteId, Sprite>();
			m_sharedpalette = sharedpalette;
		}

		public override String ToString()
		{
			return Filepath;
		}

		Boolean TryGetSpriteData(SpriteId id, out SpriteFileData data, out Int32 dataindex)
		{
			data = null;
			dataindex = Int32.MinValue;

			if (id == SpriteId.Invalid) return false;

			Int32 index = m_collection.GetIndex(id);
			if (index == Int32.MinValue) return false;

			SpriteFileData sfd = m_collection.GetData(index);
			if (sfd == null) return false;

			data = sfd;
			dataindex = index;
			return true;
		}

		public Sprite GetSprite(SpriteId id)
		{
			if (id == SpriteId.Invalid) return null;

			if (m_cachedsprites.ContainsKey(id) == true) return m_cachedsprites[id];

			SpriteFileData data;
			Int32 dataindex;
			if (TryGetSpriteData(id, out data, out dataindex) == false) return null;

			Point size;
			Texture2D pixels;
			Texture2D palette;
			Boolean ownpixels;
			Boolean ownpalette;
			Boolean paletteoverride = false;

			if (data.PcxSize > 0)
			{
				m_file.SeekFromBeginning(data.FileOffset);
				if (m_spritesystem.LoadImage(m_file, data.PcxSize, out size, out pixels, out palette) == false)
				{
					Log.Write(LogLevel.Warning, LogSystem.SpriteSystem, "Cannot load PCX image data from '{0}' for sprite #{1}", Filepath, id);

					data.IsValid = false;
					return null;
				}

				ownpixels = true;
				ownpalette = true;
			}
			else
			{
				SpriteFileData shareddata = m_collection.GetData(data.SharedIndex);
				if (shareddata == null || shareddata.Id == id) return null;

				Sprite shared_sprite = GetSprite(shareddata.Id);
				if (shared_sprite == null) return null;

				size = shared_sprite.Size;
				paletteoverride = shared_sprite.PaletteOverride;
				pixels = shared_sprite.Pixels;
				palette = shared_sprite.Palette;
				ownpixels = false;
				ownpalette = false;
			}

			if (data.CopyLastPalette == true && dataindex != 0)
			{
				SpriteFileData previousdata = m_collection.GetData(dataindex - 1);
				if (previousdata != null && previousdata.Id != id)
				{
					Sprite previoussprite = GetSprite(previousdata.Id);
					if (previoussprite != null)
					{
						ownpalette = false;
						paletteoverride = previoussprite.PaletteOverride;
						palette = previoussprite.Palette;
					}
				}
			}

			if (id == new SpriteId(0, 0) || id == SpriteId.SmallPortrait) paletteoverride = true;

			Sprite sprite = new Sprite(size, data.Axis, ownpixels, pixels, ownpalette, palette, paletteoverride);
			m_cachedsprites.Add(id, sprite);

			return sprite;
		}

		public void LoadAllSprites()
		{
			for (Int32 i = 0; i != m_collection.Count; ++i)
			{
				SpriteFileData data = m_collection.GetData(i);
				if (data == null) continue;

				GetSprite(data.Id);
			}
		}

		public Texture2D GetFirstPalette()
		{
			if (m_collection.Count == 0) return null;

			SpriteFileData data = m_collection.GetData(0);
			if (data == null) return null;

			Sprite sprite = GetSprite(data.Id);
			if (sprite == null) return null;

			return sprite.Palette;
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_cachedsprites != null)
				{
					foreach (Sprite sprite in m_cachedsprites.Values) sprite.Dispose();
					m_cachedsprites.Clear();
				}
			}

			base.Dispose(disposing);
		}

		public SpriteSystem SpriteSystem
		{
			get { return m_spritesystem; }
		}

		public String Filepath
		{
			get { return m_file.Filepath; }
		}

		public SpriteFileVersion Version
		{
			get { return m_version; }
		}

		public Boolean SharedPalette
		{
			get { return m_sharedpalette; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteSystem m_spritesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly File m_file;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_sharedpalette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteFileVersion m_version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteFileDataCollection m_collection;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<SpriteId, Sprite> m_cachedsprites;

		#endregion
	}
}