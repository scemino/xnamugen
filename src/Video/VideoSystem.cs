using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.Collections;
using System.Text;

namespace xnaMugen.Video
{
    class VideoSystem : SubSystem
    {
        public VideoSystem(SubSystems subsystems)
            : base(subsystems)
        {
            m_devicemanager = (GraphicsDeviceManager)SubSystems.Game.Services.GetService(typeof(IGraphicsDeviceService));
            m_screensize = new Point(m_devicemanager.PreferredBackBufferWidth, m_devicemanager.PreferredBackBufferHeight);
            m_camerashift = new Point(0, 0);
            m_tint = Color.White;
            m_renderer = new Renderer(this);
            m_stringbuilder = new StringBuilder();

            Device.DeviceReset += OnDeviceReset;

            OnDeviceReset(this, EventArgs.Empty);
        }

        void OnDeviceReset(Object sender, EventArgs args)
        {
            Device.VertexDeclaration = new VertexDeclaration(Device, Vertex.VertexElements);
            Device.RenderState.AlphaTestEnable = true;
            Device.RenderState.AlphaFunction = CompareFunction.Greater;
            Device.RenderState.ReferenceAlpha = 0;

            m_renderer.OnDeviceReset(sender, args);

            if (m_screenshot == null || m_screenshot.IsDisposed == true || ScreenSize != new Point(m_screenshot.Width, m_screenshot.Height))
            {
                m_screenshot = new ResolveTexture2D(Device, Device.PresentationParameters.BackBufferWidth, Device.PresentationParameters.BackBufferHeight, 1, Device.PresentationParameters.BackBufferFormat);
            }
        }

        public override void Initialize()
        {
            InitializationSettings settings = GetSubSystem<InitializationSettings>();

#if DEBUG
            ScreenSize = Mugen.ScreenSize * 2;
#else
			ScreenSize = settings.ScreenSize;
#endif

            m_renderer.UseOldShader = settings.UseOldShader;
            m_devicemanager.SynchronizeWithVerticalRetrace = settings.VSync;

            m_devicemanager.MinimumPixelShaderProfile = m_renderer.UseOldShader ? ShaderProfile.PS_2_0 : ShaderProfile.PS_3_0;
            m_devicemanager.ApplyChanges();
        }

        public void ClearScreen(Color color)
        {
            Device.Clear(color);
        }

        public void TakeScreenshot()
        {
            if (m_screenshot == null) return;

            Device.ResolveBackBuffer(m_screenshot);

            InitializationSettings settings = GetSubSystem<InitializationSettings>();

            String extension = null;
            ImageFileFormat format = ImageFileFormat.Bmp;

            switch (settings.ScreenShotFormat)
            {
                case ScreenShotFormat.Bmp:
                    extension = "bmp";
                    format = ImageFileFormat.Bmp;
                    break;

                case ScreenShotFormat.Jpg:
                    extension = "jpg";
                    format = ImageFileFormat.Jpg;
                    break;

                case ScreenShotFormat.Png:
                    extension = "png";
                    format = ImageFileFormat.Png;
                    break;

                default:
                    return;
            }

            m_stringbuilder.Length = 0;
            m_stringbuilder.AppendFormat(@"Screenshot {0:u}.{1}", DateTime.Now, extension).Replace(':', '-');

            m_screenshot.Save(m_stringbuilder.ToString(), format);
        }

        public Texture2D CloneTexture(Texture2D texture)
        {
            if (texture == null) throw new ArgumentNullException("texture");
            if (texture.Format != SurfaceFormat.Alpha8 && texture.Format != SurfaceFormat.Color) throw new ArgumentException("Cannot copy texture with format: " + texture.Format);

            Texture2D clone = new Texture2D(Device, texture.Width, texture.Height, 1, TextureUsage.None, texture.Format);
            SharedBuffer buffer = GetSubSystem<SharedBuffer>();
            Int32 buffersize = texture.Width * texture.Height * GetFormatSize(texture.Format);

            lock (buffer.LockObject)
            {
                buffer.EnsureSize(buffersize);

                texture.GetData<Byte>(buffer.Buffer, 0, buffersize);
                clone.SetData<Byte>(buffer.Buffer, 0, buffersize, SetDataOptions.None);
            }

            return clone;
        }

        public void CopyTexture(Texture2D input, Texture2D output)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (output == null) throw new ArgumentNullException("output");

            if (input.Width != output.Width || input.Height != output.Height) throw new ArgumentException("Textures are not same size");
            if (input.Format != output.Format) throw new ArgumentException("Textures do not have same SurfaceFormat");
            if (input.Format != SurfaceFormat.Alpha8 && input.Format != SurfaceFormat.Color) throw new ArgumentException("Cannot copy texture with format: " + input.Format);

            SharedBuffer buffer = GetSubSystem<SharedBuffer>();
            Int32 buffersize = input.Width * input.Height * GetFormatSize(input.Format);

            lock (buffer.LockObject)
            {
                buffer.EnsureSize(buffersize);

                input.GetData<Byte>(buffer.Buffer, 0, buffersize);
                output.SetData<Byte>(buffer.Buffer, 0, buffersize, SetDataOptions.None);
            }
        }

        public Texture2D CreatePixelTexture(Point size)
        {
            Texture2D texture = new Texture2D(Device, size.X, size.Y, 1, TextureUsage.None, SurfaceFormat.Alpha8);
            return texture;
        }

        public Texture2D CreatePaletteTexture()
        {
            Texture2D texture = new Texture2D(Device, 256, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            return texture;
        }

        public Texture2D CreateTexture(Drawing.Pixels pixels, Drawing.Palette palette)
        {
            if (pixels == null) throw new ArgumentNullException("pixels");
            if (palette == null) throw new ArgumentNullException("palette");

            Texture2D texture = new Texture2D(Device, pixels.Size.X, pixels.Size.Y, 1, TextureUsage.None, SurfaceFormat.Color);

            SharedBuffer sharedbuffer = GetSubSystem<SharedBuffer>();
            lock (sharedbuffer)
            {
                sharedbuffer.EnsureSize(pixels.Size.X * pixels.Size.Y * 4);
                Byte[] buffer = sharedbuffer.Buffer;

				Int32 imagelength = pixels.Size.X * pixels.Size.Y;
				for (Int32 i = 0; i != imagelength; ++i)
				{
					Int32 colorindex = pixels.Buffer[i];
					Color color = palette.Buffer[colorindex];
					Int32 bufferindex = i * 4;

					buffer[bufferindex + 0] = color.B;
					buffer[bufferindex + 1] = color.G;
					buffer[bufferindex + 2] = color.R;
					buffer[bufferindex + 3] = (colorindex != 0) ? color.A : (Byte)0;
				}

                texture.SetData<Byte>(buffer, 0, pixels.Size.X * pixels.Size.Y * 4, SetDataOptions.None);
            }

            return texture;
        }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            if (disposing == true)
            {
                if (m_renderer != null) m_renderer.Dispose();

                if (m_screenshot != null) m_screenshot.Dispose();
            }
        }

        public void Draw(DrawState drawstate)
        {
            if (drawstate == null) throw new ArgumentNullException("drawstate");

            m_renderer.Draw(drawstate);
        }

        static Int32 GetFormatSize(SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.Alpha8:
                    return 1;

                case SurfaceFormat.Color:
                    return 4;

                default:
                    throw new ArgumentException("Cannot get size of SurfaceFormat: " + format);
            }
        }

        public GraphicsDevice Device
        {
            get { return m_devicemanager.GraphicsDevice; }
        }

        public Point ScreenSize
        {
            get { return m_screensize; }

            set
            {
                if (m_screensize == value) return;

                m_screensize = value;

                m_devicemanager.PreferredBackBufferWidth = m_screensize.X;
                m_devicemanager.PreferredBackBufferHeight = m_screensize.Y;
                m_devicemanager.ApplyChanges();
            }
        }

        public Vector2 ScreenScale
        {
            get { return (Vector2)ScreenSize / (Vector2)Mugen.ScreenSize; }
        }

        public Point CameraShift
        {
            get { return m_camerashift; }

            set { m_camerashift = value; }
        }

        public Color Tint
        {
            get { return m_tint; }

            set { m_tint = value; }
        }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly GraphicsDeviceManager m_devicemanager;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Point m_screensize;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Point m_camerashift;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Color m_tint;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Renderer m_renderer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ResolveTexture2D m_screenshot;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly StringBuilder m_stringbuilder;

        #endregion
    }
}