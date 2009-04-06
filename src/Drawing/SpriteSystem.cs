using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;

namespace xnaMugen.Drawing
{
	class SpriteSystem : SubSystem
	{
		public SpriteSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_spritefiles = new KeyedCollection<String, SpriteFile>(x => x.Filepath, StringComparer.OrdinalIgnoreCase);
			m_palettefilecache = new Dictionary<String, Palette>(StringComparer.OrdinalIgnoreCase);
			m_fontcache = new KeyedCollection<String, Font>(x => x.Filepath, StringComparer.OrdinalIgnoreCase);
			m_fontlinemapregex = new Regex("(\\S+)\\s?(\\d+)?\\s?(\\d+)?", RegexOptions.IgnoreCase);
			m_loader = new PcxLoader(this);
		}

		public SpriteManager CreateManager(String filepath, Boolean useoverride)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			return new SpriteManager(GetSpriteFile(filepath), useoverride);
		}

		public Font LoadFont(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			if (m_fontcache.Contains(filepath) == true) return m_fontcache[filepath];

			Point size = new Point(0, 0);
			Pixels pixels = null;
			Palette palette = null;
			IO.TextFile textfile = null;

			using (IO.File file = GetSubSystem<IO.FileSystem>().OpenFile(filepath))
			{
				String sig = file.ReadString(11);
				Int32 unknown = file.ReadInt32();
				Int32 imageoffset = file.ReadInt32();
				Int32 imagesize = file.ReadInt32();

				file.SeekFromBeginning(imageoffset);
				LoadImage(file, imagesize, out size, out pixels, out palette);

				file.SeekFromBeginning(imageoffset + imagesize);
				textfile = GetSubSystem<IO.FileSystem>().BuildTextFile(file);
			}

			IO.TextSection data = textfile.GetSection("Def");
			IO.TextSection textmap = textfile.GetSection("Map");

			Int32 colors = data.GetAttribute<Int32>("colors");
			Point defaultcharsize = data.GetAttribute<Point>("size");

			ReadOnlyList<Texture2D> textures = BuildFontTextures(pixels, palette, colors);

			Int32 numchars = 0;
			Dictionary<Char, Rectangle> sizemap = new Dictionary<Char, Rectangle>();
			foreach (String line in textmap.Lines)
			{
				Match m = m_fontlinemapregex.Match(line);
				if (m.Success == false) continue;

				Char c = GetChar(m.Groups[1].Value);
				Point offset = (m.Groups[2].Value == "") ? new Point(defaultcharsize.X * numchars, 0) : new Point(Int32.Parse(m.Groups[2].Value), 0);
				Point charsize = (m.Groups[3].Value == "") ? defaultcharsize : new Point(Int32.Parse(m.Groups[3].Value), pixels.Size.Y);

				if (sizemap.ContainsKey(c) == false)
				{
					Rectangle r = new Rectangle(offset.X, offset.Y, charsize.X, charsize.Y);
					sizemap.Add(c, r);
				}

				++numchars;
			}

			Font font = new Font(this, filepath, textures, new ReadOnlyDictionary<Char, Rectangle>(sizemap), defaultcharsize, colors);
			m_fontcache.Add(font);

			return font;
		}

		ReadOnlyList<Texture2D> BuildFontTextures(Pixels pixels, Palette palette, Int32 colors)
		{
			if (pixels == null) throw new ArgumentNullException("pixels");
			if (palette == null) throw new ArgumentNullException("palette");
			if (colors <= 0) throw new ArgumentOutOfRangeException("colors");

			Int32 number = 1 + (255 / colors);
			List<Texture2D> textures = new List<Texture2D>(colors);

			for (Int32 i = 0; i != colors; ++i)
			{
				Color[] palettecolors = new Color[256];
				for (Int32 index = 0; index != number; ++index) palettecolors[255 - index] = palette.Buffer[255 - index - (i * number)];

				Palette subpalette = new Palette(palettecolors);
				textures.Add(GetSubSystem<Video.VideoSystem>().CreateTexture(pixels, subpalette));
			}

			return new ReadOnlyList<Texture2D>(textures);
		}

		SpriteFile GetSpriteFile(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			if (m_spritefiles.Contains(filepath) == true) return m_spritefiles[filepath];

			SpriteFile spritefile = CreateSpriteFile(filepath);
			m_spritefiles.Add(spritefile);

			return spritefile;
		}

		SpriteFile CreateSpriteFile(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			IO.File file = GetSubSystem<IO.FileSystem>().OpenFile(filepath);

			String signature = file.ReadString(11);
			Byte version_high = file.ReadByte();
			Byte version_low1 = file.ReadByte();
			Byte version_low2 = file.ReadByte();
			Byte version_low3 = file.ReadByte();
			Int32 numberofgroups = file.ReadInt32();
			Int32 numberofimages = file.ReadInt32();
			Int32 firstsubheader_offset = file.ReadInt32();
			Int32 subheader_size = file.ReadInt32();
			Boolean shared_palette = file.ReadByte() > 0;

			SpriteFileVersion version = new SpriteFileVersion(version_high, version_low1, version_low2, version_low3);

			List<SpriteFileData> datalist = new List<SpriteFileData>(numberofimages);

			Int32 subheader_offset = firstsubheader_offset;
			for (file.SeekFromBeginning(subheader_offset); file.ReadPosition != file.FileLength; file.SeekFromBeginning(subheader_offset))
			{
				Int32 nextsubheader_offset = file.ReadInt32();
				Int32 image_size = file.ReadInt32();
				Int16 axis_x = file.ReadInt16();
				Int16 axis_y = file.ReadInt16();
				Int16 groupnumber = file.ReadInt16();
				Int16 imagenumber = file.ReadInt16();
				Int16 sharedindex = file.ReadInt16();
				Boolean copylastpalette = file.ReadByte() > 0;

				SpriteFileData data = new SpriteFileData((Int32)file.ReadPosition + 13, image_size, new Point(axis_x, axis_y), new SpriteId(groupnumber, imagenumber), sharedindex, copylastpalette);
				datalist.Add(data);

				subheader_offset = nextsubheader_offset;
			}

			return new SpriteFile(this, file, version, datalist, shared_palette);
		}

		public Boolean LoadImage(IO.File file, Int32 pcxsize, out Point size, out Texture2D pixels, out Texture2D palette)
		{
			if (file == null) throw new ArgumentNullException("file");

			return m_loader.Load(file, pcxsize, out size, out pixels, out palette);
		}

		public Boolean LoadImage(IO.File file, Int32 pcxsize, out Point size, out Pixels pixels, out Palette palette)
		{
			if (file == null) throw new ArgumentNullException("file");

			return m_loader.Load(file, pcxsize, out size, out pixels, out palette);
		}

		public Palette LoadPaletteFile(String filepath)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");

			Palette palette;
			if (m_palettefilecache.TryGetValue(filepath, out palette) == true) return palette;

			Color[] colors = new Color[256];

			using (IO.File file = GetSubSystem<IO.FileSystem>().OpenFile(filepath))
			{
				if (file.FileLength != 768)
				{
					Log.Write(LogLevel.Error, LogSystem.SpriteSystem, "{0} is not a character palette file", filepath);
				}
				else
				{
					for (Int32 i = 255; i != -1; --i)
					{
						Byte red = file.ReadByte();
						Byte green = file.ReadByte();
						Byte blue = file.ReadByte();
						Byte alpha = (i != 0) ? (Byte)255 : (Byte)0;

						colors[i] = new Color(red, green, blue, alpha);
					}
				}
			}

			palette = new Palette(colors);
			m_palettefilecache.Add(filepath, palette);

			return palette;
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
			}

			base.Dispose(disposing);
		}

		static Char GetChar(String str)
		{
			if (str.Length == 1) return str[0];

			if (str[0] == '0' && (str[1] == 'x' || str[1] == 'X')) str = str.Remove(0, 2);

			Int32 val = Int32.Parse(str, System.Globalization.NumberStyles.HexNumber);
			return (Char)val;
		}

		#region Fields

		KeyedCollection<String, SpriteFile> m_spritefiles;

		KeyedCollection<String, Font> m_fontcache;

		Dictionary<String, Palette> m_palettefilecache;

		readonly Regex m_fontlinemapregex;

		readonly PcxLoader m_loader;

		#endregion
	}
}