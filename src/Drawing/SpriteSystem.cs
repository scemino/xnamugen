using System;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;

namespace xnaMugen.Drawing
{
	internal class SpriteSystem : SubSystem
	{
		public SpriteSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_spritefiles = new KeyedCollection<string, SpriteFile>(x => x.Filepath, StringComparer.OrdinalIgnoreCase);
			m_palettefilecache = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
			m_fontcache = new KeyedCollection<string, Font>(x => x.Filepath, StringComparer.OrdinalIgnoreCase);
			m_fontlinemapregex = new Regex("(\\S+)\\s?(\\d+)?\\s?(\\d+)?", RegexOptions.IgnoreCase);
			m_loader = new PcxLoader(this);
		}

		public SpriteManager CreateManager(string filepath)
		{
			return new SpriteManager(GetSpriteFile(filepath));
		}

		public Font LoadFont(string filepath)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));

			if (m_fontcache.Contains(filepath)) return m_fontcache[filepath];

			Point size;
			Texture2D pixels;
			Texture2D palette;
			IO.TextFile textfile;

			using (var file = GetSubSystem<IO.FileSystem>().OpenFile(filepath))
			{
				var header = new IO.FileHeaders.FontFileHeader(file);

				file.SeekFromBeginning(header.ImageOffset);
				LoadImage(file, header.ImageSize, out size, out pixels, out palette);

				file.SeekFromBeginning(header.ImageOffset + header.ImageSize);
				textfile = GetSubSystem<IO.FileSystem>().BuildTextFile(file);
			}

			var sprite = new Sprite(size, new Point(0, 0), true, pixels, true, palette, false);

			var data = textfile.GetSection("Def");
			var textmap = textfile.GetSection("Map");

			var colors = data.GetAttribute<int>("colors");
			var defaultcharsize = data.GetAttribute<Point>("size");

			var numchars = 0;
			var sizemap = new Dictionary<char, Rectangle>();
			foreach (var line in textmap.Lines)
			{
				var m = m_fontlinemapregex.Match(line);
				if (m.Success == false) continue;

				var c = GetChar(m.Groups[1].Value);
				var offset = m.Groups[2].Value == "" ? new Point(defaultcharsize.X * numchars, 0) : new Point(int.Parse(m.Groups[2].Value), 0);
				var charsize = m.Groups[3].Value == "" ? defaultcharsize : new Point(int.Parse(m.Groups[3].Value), sprite.Size.Y);

				if (sizemap.ContainsKey(c) == false)
				{
					var r = new Rectangle(offset.X, offset.Y, charsize.X, charsize.Y);
					sizemap.Add(c, r);
				}

				++numchars;
			}

			var font = new Font(this, filepath, sprite, new ReadOnlyDictionary<char, Rectangle>(sizemap), defaultcharsize, colors);
			m_fontcache.Add(font);

			return font;
		}

		private SpriteFile GetSpriteFile(string filepath)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));

			if (m_spritefiles.Contains(filepath)) return m_spritefiles[filepath];

			var spritefile = CreateSpriteFile(filepath);
			m_spritefiles.Add(spritefile);

			return spritefile;
		}

		private SpriteFile CreateSpriteFile(string filepath)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));

			var file = GetSubSystem<IO.FileSystem>().OpenFile(filepath);

			var header = new IO.FileHeaders.SpriteFileHeader(file);

			var datalist = new List<SpriteFileData>(header.NumberOfImages);

			var subheaderOffset = header.SubheaderOffset;
			for (file.SeekFromBeginning(subheaderOffset); file.ReadPosition != file.FileLength; file.SeekFromBeginning(subheaderOffset))
			{
				var subheader = new IO.FileHeaders.SpriteFileSubHeader(file);

				var data = new SpriteFileData((int)file.ReadPosition + 13, subheader.ImageSize, subheader.Axis, subheader.Id, subheader.SharedIndex, subheader.CopyLastPalette);
				datalist.Add(data);

				subheaderOffset = subheader.NextOffset;
			}

			return new SpriteFile(this, file, header.Version, datalist, header.SharedPalette);
		}

		public bool LoadImage(IO.File file, int pcxsize, out Point size, out Texture2D pixels, out Texture2D palette)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			return m_loader.Load(file, pcxsize, out size, out pixels, out palette);
		}

		public Texture2D LoadPaletteFile(string filepath)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));

			Texture2D palette;
			if (m_palettefilecache.TryGetValue(filepath, out palette)) return palette;

			palette = GetSubSystem<Video.VideoSystem>().CreatePaletteTexture();
			m_palettefilecache.Add(filepath, palette);

			using (var file = GetSubSystem<IO.FileSystem>().OpenFile(filepath))
			{
				if (file.FileLength != 256 * 3)
				{
					Log.Write(LogLevel.Error, LogSystem.SpriteSystem, "{0} is not a character palette file", filepath);
				}
				else
				{
					var buffer = new byte[256 * 4];
					var filedata = file.ReadBytes(256 * 3);

					for (var i = 0; i != 256; ++i)
					{
						var bufferindex = (255 - i) * 4;
						var fileindex = i * 3;

						buffer[bufferindex + 0] = filedata[fileindex + 2];
						buffer[bufferindex + 1] = filedata[fileindex + 1];
						buffer[bufferindex + 2] = filedata[fileindex + 0];
						buffer[bufferindex + 3] = 255;
					}

					palette.SetData(buffer, 0, 256 * 4);
				}
			}

			return palette;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_spritefiles != null) foreach (var spritefile in m_spritefiles) spritefile.Dispose();

				if (m_palettefilecache != null) foreach (var texture in m_palettefilecache.Values) texture.Dispose();
			}

			base.Dispose(disposing);
		}

		private static char GetChar(string str)
		{
			if (str.Length == 1) return str[0];

			if (str[0] == '0' && (str[1] == 'x' || str[1] == 'X')) str = str.Remove(0, 2);

			var val = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
			return (char)val;
		}

		#region Fields

		private KeyedCollection<string, SpriteFile> m_spritefiles;

		private KeyedCollection<string, Font> m_fontcache;

		private Dictionary<string, Texture2D> m_palettefilecache;

		private readonly Regex m_fontlinemapregex;

		private readonly PcxLoader m_loader;

		#endregion
	}
}