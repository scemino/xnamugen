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

		public Boolean Load(IO.File file, Int32 pcxsize, out Point size, out Texture2D pixels, out Texture2D palette)
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
					palette = m_system.GetSubSystem<Video.VideoSystem>().CreatePaletteTexture();
				}

				return true;
			}
		}

		Int32 ReadFile(IO.File file, Int32 count)
		{
			if (file == null) throw new ArgumentNullException("file");
			if (count <= 0) throw new ArgumentOutOfRangeException("count");

			if (m_filebuffer.Length < count) m_filebuffer = new Byte[count];

			return file.ReadBytes(m_filebuffer, count);
		}

		Texture2D LoadPixels(Point size, Int32 bytesperline, ref Int32 readoffset)
		{
			SharedBuffer sharedbuffer = m_system.GetSubSystem<SharedBuffer>();
			lock (sharedbuffer.LockObject)
			{
				readoffset = IO.FileHeaders.PCX.HeaderSize;

				sharedbuffer.EnsureSize(size.X * size.Y);
				Byte[] buffer = sharedbuffer.Buffer;

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
		}

		Texture2D LoadPalette(Int32 pcxsize, IO.FileHeaders.PCX header, ref Int32 readoffset)
		{
			Texture2D texture = m_system.GetSubSystem<Video.VideoSystem>().CreatePaletteTexture();
			SharedBuffer buffer = m_system.GetSubSystem<SharedBuffer>();

			lock (buffer.LockObject)
			{
				buffer.EnsureSize(256 * 4);

				for (Int32 i = 0; i != 256; ++i)
				{
					Int32 writeoffset = i * 4;

					buffer[writeoffset + 0] = m_filebuffer[readoffset + 2];
					buffer[writeoffset + 1] = m_filebuffer[readoffset + 1];
					buffer[writeoffset + 2] = m_filebuffer[readoffset + 0];
					buffer[writeoffset + 3] = 255;

					readoffset += 3;
				}

				texture.SetData<Byte>(buffer.Buffer, 0, 256 * 4, SetDataOptions.None);
				return texture;
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Byte[] m_filebuffer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteSystem m_system;

		#endregion
	}
}