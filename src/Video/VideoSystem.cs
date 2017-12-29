using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System.IO;

namespace xnaMugen.Video
{
	internal class VideoSystem :	SubSystem
	{
		public VideoSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_devicemanager	= (GraphicsDeviceManager)SubSystems.Game.Services.GetService(typeof(IGraphicsDeviceService));
			m_screensize = new Point(m_devicemanager.PreferredBackBufferWidth, m_devicemanager.PreferredBackBufferHeight);
			m_camerashift =	new	Point(0, 0);
			m_tint = Color.White;
			m_renderer = new Renderer(this);
			m_stringbuilder	= new StringBuilder();

			Device.DeviceReset += OnDeviceReset;

			OnDeviceReset(this,	EventArgs.Empty);
		}

		private void OnDeviceReset(object sender, EventArgs	args)
		{
			Device.BlendState = BlendState.AlphaBlend;

			m_renderer.OnDeviceReset(sender, args);

			if (m_screenshot ==	null ||	m_screenshot.IsDisposed	|| ScreenSize != new Point(m_screenshot.Width, m_screenshot.Height))
			{
				m_screenshot = new RenderTarget2D(Device, Device.PresentationParameters.BackBufferWidth, Device.PresentationParameters.BackBufferHeight, true, Device.PresentationParameters.BackBufferFormat, Device.PresentationParameters.DepthStencilFormat);
			}
		}

		public override	void Initialize()
		{
			var settings	= GetSubSystem<InitializationSettings>();

#if	FRANTZX
			ScreenSize = Mugen.ScreenSize *	2;
#else
			ScreenSize = settings.ScreenSize;
#endif

			m_renderer.UseOldShader	= settings.UseOldShader;
			m_devicemanager.SynchronizeWithVerticalRetrace = settings.VSync;

			//m_devicemanager.MinimumPixelShaderProfile	= m_renderer.UseOldShader ?	ShaderProfile.PS_2_0 : ShaderProfile.PS_3_0;
			m_devicemanager.ApplyChanges();
		}

		public void	ClearScreen(Color color)
		{
			Device.Clear(color);
		}

		public void	TakeScreenshot()
		{
			if (m_screenshot ==	null) return;

			Device.SetRenderTarget(m_screenshot);

			var settings	= GetSubSystem<InitializationSettings>();

			string extension = null;
			/*ImageFileFormat format = ImageFileFormat.Bmp;

			switch (settings.ScreenShotFormat)
			{
				case ScreenShotFormat.Bmp:
					extension =	"bmp";
					format = ImageFileFormat.Bmp;
					break;

				case ScreenShotFormat.Jpg:
					extension =	"jpg";
					format = ImageFileFormat.Jpg;
					break;

				case ScreenShotFormat.Png:
					extension =	"png";
					format = ImageFileFormat.Png;
					break;

				default:
					return;
			}*/

			switch (settings.ScreenShotFormat)
			{
				case ScreenShotFormat.Png:
					extension =	"png";
					break;
				case ScreenShotFormat.Jpg:
					extension =	"jpg";
					break;
				default:
					return;
			}

			m_stringbuilder.Length = 0;
			m_stringbuilder.AppendFormat(@"Screenshot {0:u}.{1}", DateTime.Now,	extension).Replace(':',	'-');

			using (var fs =	File.OpenWrite(m_stringbuilder.ToString()))
			{
				switch (settings.ScreenShotFormat)
				{
					case ScreenShotFormat.Png:
						m_screenshot.SaveAsPng(fs, settings.ScreenSize.X, settings.ScreenSize.Y);
						break;
					case ScreenShotFormat.Jpg:
						m_screenshot.SaveAsJpeg(fs, settings.ScreenSize.X, settings.ScreenSize.Y);
						break;
					default:
						return;
				}
			}
		}

		public Texture2D CreatePixelTexture(Point size)
		{
			var texture =	new	Texture2D(Device, size.X, size.Y, false, SurfaceFormat.Alpha8);
			return texture;
		}

		public Texture2D CreatePaletteTexture()
		{
			var texture =	new	Texture2D(Device, 256, 1, false, SurfaceFormat.Color);
			return texture;
		}

		protected override void	Dispose(bool	disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				m_renderer?.Dispose();
				m_screenshot?.Dispose();
			}
		}

		public void	Draw(DrawState drawstate)
		{
			if (drawstate == null) throw new ArgumentNullException(nameof(drawstate));

			m_renderer.Draw(drawstate);
		}

		public GraphicsDevice Device => m_devicemanager.GraphicsDevice;

		public Point ScreenSize
		{
			get => m_screensize;

			set
			{
				if (m_screensize ==	value) return;

				m_screensize = value;

				m_devicemanager.PreferredBackBufferWidth = m_screensize.X;
				m_devicemanager.PreferredBackBufferHeight =	m_screensize.Y;
				m_devicemanager.ApplyChanges();
			}
		}

		public Vector2 ScreenScale => (Vector2)ScreenSize / (Vector2)Mugen.ScreenSize;

		public Point CameraShift
		{
			get => m_camerashift;

			set => m_camerashift	= value;
		}

		public Color Tint
		{
			get => m_tint;

			set => m_tint = value;
		}

		#region	Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly GraphicsDeviceManager m_devicemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Point m_screensize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Point m_camerashift;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Color m_tint;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Renderer m_renderer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private RenderTarget2D m_screenshot;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StringBuilder m_stringbuilder;

		#endregion
	}
}