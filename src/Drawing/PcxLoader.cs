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
		}

		public Boolean Load(IO.File file, Int32 pcxsize, out Point size, out Texture2D pixels, out Texture2D palette)
		{
			if (file == null) throw new ArgumentNullException("file");

			size = new Point(Int32.MinValue, Int32.MinValue);
			pixels = null;
			palette = null;

			Byte[] filedata = file.ReadBytes(pcxsize);
			if (filedata.Length != pcxsize) return false;

			IO.FileHeaders.PcxFileHeader header = new IO.FileHeaders.PcxFileHeader(filedata);
			Point tempsize = new Point(header.XMax - header.XMin + 1, header.YMax - header.YMin + 1);

			if (header.Manufacturer != 10 || header.Encoding != 1 || header.Version > 5 || header.BitsPerPixel != 8) return false;
			if (header.ColorPlanes != 1) return false;
			if (tempsize.X <= 0 || tempsize.Y <= 0) return false;

			size = tempsize;

			Int32 readoffset = 0;
			pixels = LoadPixels(filedata, size, header.BytesPerLine, ref readoffset);
			palette = LoadPalette(filedata, ref readoffset);
			return true;
		}

		Texture2D LoadPixels(Byte[] filedata, Point size, Int32 bytesperline, ref Int32 readoffset)
		{
			if (filedata == null) throw new ArgumentNullException("filedata");

			readoffset = IO.FileHeaders.PcxFileHeader.HeaderSize;

			Byte[] buffer = new Byte[size.X * size.Y + 1];

			for (Int32 y = 0; y != size.Y; ++y)
			{
				Int32 offset = y * size.X;

				for (Int32 x = 0; x < bytesperline; )
				{
					Byte data = filedata[readoffset++];
					if (data > 192)
					{
						data -= 192;
						Byte color = filedata[readoffset++];

						if (x <= size.X)
						{
							for (Byte repeat = 0; repeat < data; ++repeat)
							{
								buffer[offset++] = color;
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
						if (x <= size.X) buffer[offset++] = data;
						++x;
					}
				}
			}

			Texture2D texture = m_system.GetSubSystem<Video.VideoSystem>().CreatePixelTexture(size);
			texture.SetData<Byte>(buffer, 0, size.X * size.Y, SetDataOptions.None);
			return texture;
		}

		Texture2D LoadPalette(Byte[] filedata, ref Int32 offset)
		{
			if (filedata == null) throw new ArgumentNullException("filedata");

			Texture2D texture = m_system.GetSubSystem<Video.VideoSystem>().CreatePaletteTexture();

			if (filedata[offset++] == 12 && filedata.Length - offset >= (256 * 3))
			{
				Byte[] buffer = new Byte[256 * 4];

				for (Int32 i = 0; i != 256; ++i, offset += 3)
				{
					Int32 writeoffset = i * 4;

					buffer[writeoffset + 0] = filedata[offset + 2];
					buffer[writeoffset + 1] = filedata[offset + 1];
					buffer[writeoffset + 2] = filedata[offset + 0];
					buffer[writeoffset + 3] = 255;
				}

				texture.SetData<Byte>(buffer, 0, 256 * 4, SetDataOptions.None);
			}

			return texture;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteSystem m_system;

		#endregion
	}
}