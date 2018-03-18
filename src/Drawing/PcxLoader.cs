using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Drawing
{
	internal class PcxLoader
	{
		public PcxLoader(SpriteSystem system)
		{
			if (system == null) throw new ArgumentNullException(nameof(system));

			m_system = system;
		}

		public bool Load(IO.File file, int pcxsize, out Point size, out Texture2D pixels, out Texture2D palette)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			size = new Point(int.MinValue, int.MinValue);
			pixels = null;
			palette = null;

			var filedata = file.ReadBytes(pcxsize);
			
			if (filedata.Length != pcxsize) return false;

			var header = new IO.FileHeaders.PcxFileHeader(filedata);
			var tempsize = new Point(header.XMax - header.XMin + 1, header.YMax - header.YMin + 1);

			if (header.Manufacturer != 10 || header.Encoding != 1 || header.Version > 5 || header.BitsPerPixel != 8) return false;
			if (header.ColorPlanes != 1) return false;
			if (tempsize.X <= 0 || tempsize.Y <= 0) return false;

			size = tempsize;

			var readoffset = 0;
			pixels = LoadPixels(filedata, size, header.BytesPerLine, ref readoffset);
			palette = LoadPalette(filedata, ref readoffset);
			return true;
		}

		private Texture2D LoadPixels(byte[] filedata, Point size, int bytesperline, ref int readoffset)
		{
			if (filedata == null) throw new ArgumentNullException(nameof(filedata));

			readoffset = IO.FileHeaders.PcxFileHeader.HeaderSize;

			var buffer = new byte[size.X * size.Y + 1];

			for (var y = 0; y != size.Y; ++y)
			{
				var offset = y * size.X;

				for (var x = 0; x < bytesperline; )
				{
					var data = filedata[readoffset++];
					if (data > 192)
					{
						data -= 192;
						var color = filedata[readoffset++];

						if (x <= size.X)
						{
							for (byte repeat = 0; repeat < data; ++repeat)
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

			var texture = m_system.GetSubSystem<Video.VideoSystem>().CreatePixelTexture(size);
			texture.SetData(buffer, 0, size.X * size.Y);
			return texture;
		}

		private Texture2D LoadPalette(byte[] filedata, ref int offset)
		{
			if (filedata == null) throw new ArgumentNullException(nameof(filedata));

			var texture = m_system.GetSubSystem<Video.VideoSystem>().CreatePaletteTexture();

			if (filedata[offset++] == 12 && filedata.Length - offset >= 256 * 3)
			{
				var buffer = new byte[256 * 4];

				for (var i = 0; i != 256; ++i, offset += 3)
				{
					var writeoffset = i * 4;

					buffer[writeoffset + 0] = filedata[offset + 2];
					buffer[writeoffset + 1] = filedata[offset + 1];
					buffer[writeoffset + 2] = filedata[offset + 0];
					buffer[writeoffset + 3] = 255;
				}

				texture.SetData(buffer, 0, 256 * 4);
			}

			return texture;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteSystem m_system;

		#endregion
	}
}