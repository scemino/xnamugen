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

        public Texture2D CreatePixelTexture(Point size)
        {
            Texture2D texture = new Texture2D(Device, size.X, size.Y, 1, TextureUsage.None, SurfaceFormat.Single);
            return texture;
        }

        public Texture2D CreatePaletteTexture()
        {
            Texture2D texture = new Texture2D(Device, 256, 1, 1, TextureUsage.None, SurfaceFormat.Color);
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

				case SurfaceFormat.Single:
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