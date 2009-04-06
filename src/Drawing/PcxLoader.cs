using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Drawing
{
	class PcxLoader
	{
		public PcxLoader(SpriteSystem system)
		{
			if (system == null) throw new ArgumentNullException("system");

			m_system = system;
			m_filebuffer = new Byte[1];
		}

		public Boolean Load(IO.File file, Int32 pcxsize, out Point size, out Pixels pixels, out Palette palette)
		{
			if (file == null) throw new ArgumentNullException("file");

			size = new Point(Int32.MinValue, Int32.MinValue);
			pixels = null;
			palette = null;

			lock (m_filebuffer.SyncRoot)
			{
				Int32 bytesread = ReadFile(file, pcxsize);

				IO.FileHeaders.PCX header = new IO.FileHeaders.PCX(m_filebuffer);
				Point tempsize = new Point(header.XMax - header.XMin + 1, header.YMax - header.YMin + 1);

				if (header.Manufacturer != 10 || header.Encoding != 1 || header.Version > 5 || header.BitsPerPixel != 8) return false;
				if (header.ColorPlanes != 1) return false;
				if (tempsize.X <= 0 || tempsize.Y <= 0) return false;

				size = tempsize;

				Int32 readoffset = 0;
				pixels = LoadPixels(size, header.BytesPerLine, ref readoffset);

				if (m_filebuffer[readoffset++] == 12 && m_filebuffer.Length - readoffset >= (256 * 3))
				{
					palette = LoadPalette(pcxsize, header, ref readoffset);
				}
				else
				{
					palette = new Palette();
				}

				return true;
			}
		}

		public Boolean Load(IO.File file, Int32 pcxsize, out Point size, out Texture2D pixeltexture, out Texture2D palettetexture)
		{
			if (file == null) throw new ArgumentNullException("file");

			Pixels pixels;
			Palette palette;

			if (Load(file, pcxsize, out size, out pixels, out palette) == true)
			{
				pixeltexture = m_system.GetSubSystem<Video.VideoSystem>().CreatePixelTexture(pixels.Size);
				pixeltexture.SetData<Byte>(pixels.Buffer);

				palettetexture = m_system.GetSubSystem<Video.VideoSystem>().CreatePaletteTexture();
				palettetexture.SetData<Color>(palette.Buffer);

				return true;
			}
			else
			{
				pixeltexture = null;
				palettetexture = null;
				return false;
			}

		}

		Int32 ReadFile(IO.File file, Int32 count)
		{
			if (file == null) throw new ArgumentNullException("file");
			if (count <= 0) throw new ArgumentOutOfRangeException("count");

			if (m_filebuffer.Length < count) m_filebuffer = new Byte[count];

			return file.ReadBytes(m_filebuffer, count);
		}

		Pixels LoadPixels(Point size, Int32 bytesperline, ref Int32 readoffset)
		{
			readoffset = IO.FileHeaders.PCX.HeaderSize;

			Byte[] buffer = new Byte[size.X * size.Y];

			for (Int32 y = 0; y != size.Y; ++y)
			{
				Int32 offset = y * size.X;

				for (Int32 x = 0; x < bytesperline; )
				{
					Byte data = m_filebuffer[readoffset++];
					if (data > 192)
					{
						data -= 192;
						Byte color = m_filebuffer[readoffset++];

						if (x <= size.X)
						{
							for (Byte repeat = 0; repeat < data; ++repeat)
							{
								if (offset < buffer.Length) buffer[offset++] = color;
								++x;
							}
						}
						else
						{
							x += data;
						}
					}
					else
					{
						if (x <= size.X && offset < buffer.Length) buffer[offset++] = data;
						++x;
					}
				}
			}

			return new Pixels(size, buffer);
		}

		Palette LoadPalette(Int32 pcxsize, IO.FileHeaders.PCX header, ref Int32 readoffset)
		{
			Color[] colors = new Color[256];

			for (Int32 i = 0; i != 256; ++i)
			{
				Byte red = m_filebuffer[readoffset + 0];
				Byte green = m_filebuffer[readoffset + 1];
				Byte blue = m_filebuffer[readoffset + 2];
				Byte alpha = (i != 0) ? (Byte)255 : (Byte)0;

				colors[i] = new Color(red, green, blue, alpha);
				readoffset += 3;
			}

			return new Palette(colors);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Byte[] m_filebuffer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteSystem m_system;

		#endregion
	}
}