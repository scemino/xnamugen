using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;

namespace xnaMugen.Video
{
    internal class Renderer : Resource
    {
        public Renderer(VideoSystem videosystem)
        {
            if (videosystem == null) throw new ArgumentNullException(nameof(videosystem));

            m_videosystem = videosystem;

            m_videosystem.SubSystems.Game.Content.RootDirectory = "data";
            m_effect = m_videosystem.SubSystems.Game.Content.Load<Effect>("bin/shader");

            m_drawbuffer = new Vertex[500];
            m_parameters = new KeyedCollection<string, EffectParameter>(x => x.Name);

            m_nullpixels = m_videosystem.CreatePixelTexture(new Point(2, 2));
            m_nullpalette = m_videosystem.CreatePaletteTexture();

            var pixels = new byte[] { 1, 2, 1, 2 };
            m_nullpixels.SetData(pixels);

            var paldata = new Color[256];
            paldata[1] = Color.White;
            paldata[2] = Color.Red;
            m_nullpalette.SetData(paldata);
        }

        public void OnDeviceReset(object sender, EventArgs args)
        {
            var m = Matrix.Identity;
            m *= Matrix.CreateScale(1, -1, -1);
            m *= Matrix.CreateScale(ScreenScale.X, ScreenScale.Y, 1);
            m *= Matrix.CreateOrthographicOffCenter(0, ScreenSize.X, -ScreenSize.Y, 0, 0, 1);

            GetShaderParameter("xMatrix").SetValue(m);
            GetShaderParameter("xRotation").SetValue(Matrix.Identity);
            GetShaderParameter("xBlendWeight").SetValue(1.0f);

            Device.BlendState = BlendState.NonPremultiplied;
            //Device.RenderState.AlphaBlendEnable =	false;
        }

        public void Draw(DrawState drawstate)
        {
            if (drawstate == null) throw new ArgumentNullException(nameof(drawstate));

            if (drawstate.Mode == DrawMode.None) return;

            drawstate.Pixels = drawstate.Pixels ?? m_nullpixels;
            drawstate.Palette = drawstate.Palette ?? m_nullpalette;

            SetBlending(drawstate.Blending);
            SetScissorTest(drawstate.ScissorRectangle);

            switch (drawstate.Mode)
            {
                case DrawMode.Normal:
                    NormalDraw(drawstate);
                    break;

                case DrawMode.Font:
                    FontDraw(drawstate);
                    break;

                case DrawMode.OutlinedRectangle:
                    OutlinedRectangleDraw(drawstate);
                    break;

                case DrawMode.FilledRectangle:
                    FilledRectangleDraw(drawstate);
                    break;

                case DrawMode.Lines:
                    LineDraw(drawstate);
                    break;
            }
        }

        private void SetBlending(Blending blending)
        {
            switch (blending.BlendType)
            {
                case BlendType.Add:
                    GetShaderParameter("xBlendWeight").SetValue(blending.SourceFactor / 255.0f);
                    Device.BlendState = BlendState.Additive;
                    break;

                case BlendType.Subtract:
                    GetShaderParameter("xBlendWeight").SetValue(blending.SourceFactor / 255.0f);
                    Device.BlendState = new BlendState
                    {
                        ColorSourceBlend = Blend.One,
                        ColorDestinationBlend = Blend.One,
                        ColorBlendFunction = BlendFunction.ReverseSubtract,
                        AlphaSourceBlend = Blend.One,
                        AlphaDestinationBlend = Blend.One,
                        AlphaBlendFunction = BlendFunction.ReverseSubtract
                    };
                    break;

                default:
                    GetShaderParameter("xBlendWeight").SetValue(1.0f);
                    Device.BlendState = BlendState.NonPremultiplied;
                    break;
            }
        }

        private void SetScissorTest(Rectangle rectangle)
        {
            if (rectangle.IsEmpty)
            {
                var rs = new RasterizerState { ScissorTestEnable = false };
                Device.RasterizerState = rs;
            }
            else
            {
                rectangle.X = (int)(rectangle.X * ScreenScale.X);
                rectangle.Width = (int)(rectangle.Width * ScreenScale.X);

                rectangle.Y = (int)(rectangle.Y * ScreenScale.Y);
                rectangle.Height = (int)(rectangle.Height * ScreenScale.Y);

                var rs = new RasterizerState { ScissorTestEnable = true };
                Device.RasterizerState = rs;
                Device.ScissorRectangle = rectangle;
            }
        }

        private int DefaultDrawSetup(DrawState drawstate, Point pixelsize)
        {
            if (drawstate == null) throw new ArgumentNullException(nameof(drawstate));

            var count = 0;
            var camerashift = CameraShift;
            foreach (var data in drawstate)
            {
                Point drawsize;

                if (data.DrawRect != null)
                {
                    drawsize = new Point(data.DrawRect.Value.Width, data.DrawRect.Value.Height);

                    SetTextureCoords(m_drawbuffer, count * 6, pixelsize, data.DrawRect.Value, drawstate.Flip);
                }
                else
                {
                    drawsize = pixelsize;

                    SetTextureCoords(m_drawbuffer, count * 6, drawstate.Flip);
                }

                var frect = MakeVertRect(drawsize, data.Location, camerashift, drawstate.Scale, drawstate.Axis - drawstate.Offset, drawstate.Flip);
                var rotationaxis = data.Location + camerashift + drawstate.Offset;

                SetPosition(m_drawbuffer, count * 6, frect, MathHelper.ToRadians(drawstate.Rotation), rotationaxis);
                SetColor(m_drawbuffer, count * 6, Misc.BlendColors(Tint, data.Tint));

                ++count;
            }

            return count;
        }

        private void NormalDraw(DrawState drawstate)
        {
            if (drawstate == null) throw new ArgumentNullException(nameof(drawstate));
            if (drawstate.Mode != DrawMode.Normal) throw new ArgumentException("Incorrect drawstate");

            m_effect.CurrentTechnique = UseOldShader ? m_effect.Techniques["DrawOLD"] : m_effect.Techniques["Draw"];

            SetShaderParameters(drawstate.ShaderParameters, drawstate.Pixels, drawstate.Palette);

            var count = DefaultDrawSetup(drawstate, new Point(drawstate.Pixels.Width, drawstate.Pixels.Height));
            if (count > 0) FinishDrawing(PrimitiveType.TriangleList, count * 2);
        }

        private void FontDraw(DrawState drawstate)
        {
            if (drawstate == null) throw new ArgumentNullException(nameof(drawstate));
            if (drawstate.Mode != DrawMode.Font) throw new ArgumentException("Incorrect	drawstate");

            m_effect.CurrentTechnique = m_effect.Techniques["FontDraw"];

            SetShaderParameters(drawstate.ShaderParameters, drawstate.Pixels, drawstate.Palette);

            var count = DefaultDrawSetup(drawstate, new Point(drawstate.Pixels.Width, drawstate.Pixels.Height));
            if (count > 0) FinishDrawing(PrimitiveType.TriangleList, count * 2);
        }

        private void OutlinedRectangleDraw(DrawState drawstate)
        {
            if (drawstate == null) throw new ArgumentNullException(nameof(drawstate));
            if (drawstate.Mode != DrawMode.OutlinedRectangle) throw new ArgumentException("Incorrect drawstate");

            m_effect.CurrentTechnique = UseOldShader ? m_effect.Techniques["DrawOLD"] : m_effect.Techniques["Draw"];

            SetShaderParameters(drawstate.ShaderParameters, drawstate.Pixels, drawstate.Palette);

            var count = 0;
            foreach (var data in drawstate)
            {
                if (data.DrawRect != null)
                {
                    var drawsize = new Point(data.DrawRect.Value.Width, data.DrawRect.Value.Height);
                    var frect = MakeVertRect(drawsize, data.Location, CameraShift, drawstate.Scale, drawstate.Axis - drawstate.Offset, drawstate.Flip);

                    m_drawbuffer[count * 8 + 0].Position = new Vector4(frect.Left, frect.Top, 0, 1);
                    m_drawbuffer[count * 8 + 1].Position = new Vector4(frect.Left, frect.Bottom, 0, 1);
                    m_drawbuffer[count * 8 + 2].Position = new Vector4(frect.Right, frect.Top, 0, 1);
                    m_drawbuffer[count * 8 + 3].Position = new Vector4(frect.Right, frect.Bottom, 0, 1);
                    m_drawbuffer[count * 8 + 4].Position = new Vector4(frect.Left, frect.Top, 0, 1);
                    m_drawbuffer[count * 8 + 5].Position = new Vector4(frect.Right, frect.Top, 0, 1);
                    m_drawbuffer[count * 8 + 6].Position = new Vector4(frect.Left, frect.Bottom, 0, 1);
                    m_drawbuffer[count * 8 + 7].Position = new Vector4(frect.Right, frect.Bottom, 0, 1);

                    for (var i = 0; i != 8; ++i) m_drawbuffer[count * 8 + i].Tint = data.Tint;

                    ++count;
                }
            }

            if (count > 0) FinishDrawing(PrimitiveType.LineList, count * 4);
        }

        private void FilledRectangleDraw(DrawState drawstate)
        {
            if (drawstate == null) throw new ArgumentNullException(nameof(drawstate));
            if (drawstate.Mode != DrawMode.FilledRectangle) throw new ArgumentException("Incorrect drawstate");

            m_effect.CurrentTechnique = UseOldShader ? m_effect.Techniques["DrawOLD"] : m_effect.Techniques["Draw"];

            SetShaderParameters(drawstate.ShaderParameters, drawstate.Pixels, drawstate.Palette);

            var count = DefaultDrawSetup(drawstate, new Point(drawstate.Pixels.Width, drawstate.Pixels.Height));
            if (count > 0) FinishDrawing(PrimitiveType.TriangleList, count * 2);
        }

        private void LineDraw(DrawState drawstate)
        {
            if (drawstate == null) throw new ArgumentNullException(nameof(drawstate));
            if (drawstate.Mode != DrawMode.Lines) throw new ArgumentException("Incorrect drawstate");

            m_effect.CurrentTechnique = UseOldShader ? m_effect.Techniques["DrawOLD"] : m_effect.Techniques["Draw"];

            SetShaderParameters(drawstate.ShaderParameters, drawstate.Pixels, drawstate.Palette);

            var count = 0;
            var point = false;
            foreach (var data in drawstate)
            {
                if (point == false)
                {
                    point = true;

                    m_drawbuffer[count * 2 + 0] = new Vertex(new Vector4(data.Location + CameraShift, 0, 1), Vector2.Zero, data.Tint);
                }
                else
                {
                    point = false;

                    m_drawbuffer[count * 2 + 1] = new Vertex(new Vector4(data.Location + CameraShift, 0, 1), Vector2.Zero, data.Tint);
                    ++count;
                }
            }

            if (count > 0) FinishDrawing(PrimitiveType.LineList, count);
        }

        private void FinishDrawing(PrimitiveType drawtype, int count)
        {
            foreach (var pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawUserPrimitives(drawtype, m_drawbuffer, 0, count, Vertex.VertexDeclaration);
            }

            m_effect.GraphicsDevice.Textures[0] = null;
            m_effect.GraphicsDevice.Textures[1] = null;
        }

        private void SetShaderParameters(ShaderParameters parameters, Texture2D pixels, Texture2D palette)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (palette == null) throw new ArgumentNullException(nameof(palette));

            GetShaderParameter("xPixels").SetValue(pixels);
            GetShaderParameter("xPalette").SetValue(palette);

            GetShaderParameter("xFontColorIndex").SetValue(parameters.FontColorIndex);
            GetShaderParameter("xFontTotalColors").SetValue(parameters.FontTotalColors);

            if (parameters.PaletteFxEnable)
            {
                GetShaderParameter("xPalFx_Use").SetValue(true);
                GetShaderParameter("xPalFx_Add").SetValue(new Vector4(parameters.PaletteFxAdd, 0));
                GetShaderParameter("xPalFx_Mul").SetValue(new Vector4(parameters.PaletteFxMultiply, 1));
                GetShaderParameter("xPalFx_Invert").SetValue(parameters.PaletteFxInvert);
                GetShaderParameter("xPalFx_Color").SetValue(parameters.PaletteFxColor);

                var sincolor = parameters.PaletteFxSinAdd * (float)Math.Sin(parameters.PaletteFxTime * MathHelper.TwoPi / parameters.PaletteFxSinAdd.W);
                sincolor.W = 0;

                GetShaderParameter("xPalFx_SinMath").SetValue(sincolor);
            }
            else
            {
                GetShaderParameter("xPalFx_Use").SetValue(false);
            }

            if (parameters.AfterImageEnable)
            {
                GetShaderParameter("xAI_Use").SetValue(true);
                GetShaderParameter("xAI_Invert").SetValue(parameters.AfterImageInvert);
                GetShaderParameter("xAI_color").SetValue(parameters.AfterImageColor);
                GetShaderParameter("xAI_preadd").SetValue(new Vector4(parameters.AfterImagePreAdd, 0));
                GetShaderParameter("xAI_contrast").SetValue(new Vector4(parameters.AfterImageConstrast, 1));
                GetShaderParameter("xAI_postadd").SetValue(new Vector4(parameters.AfterImagePostAdd, 0));
                GetShaderParameter("xAI_paladd").SetValue(new Vector4(parameters.AfterImagePaletteAdd, 0));
                GetShaderParameter("xAI_palmul").SetValue(new Vector4(parameters.AfterImagePaletteMultiply, 1));
                GetShaderParameter("xAI_number").SetValue(parameters.AfterImageNumber);
            }
            else
            {
                GetShaderParameter("xAI_Use").SetValue(false);
            }
        }

        private EffectParameter GetShaderParameter(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (m_parameters.Contains(name)) return m_parameters[name];

            var parameter = m_effect.Parameters[name];
            m_parameters.Add(parameter);

            return parameter;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_nullpixels != null) m_nullpixels.Dispose();

                if (m_nullpalette != null) m_nullpalette.Dispose();
            }

            base.Dispose(disposing);
        }

        private static void SetPosition(Vertex[] buffer, int offset, FRect r, float rotation, Vector2 axis)
        {
            var v1 = RotatePoint(new Vector2(r.Left, r.Top), rotation, axis);
            var v2 = RotatePoint(new Vector2(r.Right, r.Bottom), rotation, axis);

            buffer[offset + 0].Position = new Vector4(v1.X, v1.Y, 0, 1);
            buffer[offset + 1].Position = new Vector4(v2.X, v1.Y, 0, 1);
            buffer[offset + 2].Position = new Vector4(v2.X, v2.Y, 0, 1);

            buffer[offset + 3].Position = new Vector4(v1.X, v1.Y, 0, 1);
            buffer[offset + 4].Position = new Vector4(v2.X, v2.Y, 0, 1);
            buffer[offset + 5].Position = new Vector4(v1.X, v2.Y, 0, 1);
        }

        private static Vector2 RotatePoint(Vector2 point, float radians, Vector2 axis)
        {
            var x = Math.Cos(radians) * (point.X - axis.X) - Math.Sin(radians) * (point.Y - axis.Y);
            var y = Math.Sin(radians) * (point.X - axis.X) + Math.Cos(radians) * (point.Y - axis.Y);

            return new Vector2((float)x + axis.X, (float)y + axis.Y);
        }

        private static void SetTextureCoords(Vertex[] buffer, int offset, Point textsize, Rectangle textrect, SpriteEffects flip)
        {
            var x1 = textrect.Left / (float)textsize.X;
            var x2 = textrect.Right / (float)textsize.X;
            var y1 = textrect.Top / (float)textsize.Y;
            var y2 = textrect.Bottom / (float)textsize.Y;

            if ((flip & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally)
            {
                var temp = x1;
                x1 = x2;
                x2 = temp;
            }

            if ((flip & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically)
            {
                var temp = y1;
                y1 = y2;
                y2 = temp;
            }

            buffer[offset + 0].TextureCoordinate = new Vector2(x1, y1);
            buffer[offset + 1].TextureCoordinate = new Vector2(x2, y1);
            buffer[offset + 2].TextureCoordinate = new Vector2(x2, y2);

            buffer[offset + 3].TextureCoordinate = new Vector2(x1, y1);
            buffer[offset + 4].TextureCoordinate = new Vector2(x2, y2);
            buffer[offset + 5].TextureCoordinate = new Vector2(x1, y2);
        }

        private static void SetTextureCoords(Vertex[] buffer, int offset, SpriteEffects flip)
        {
            SetTextureCoords(buffer, offset, new Point(1, 1), new Rectangle(0, 0, 1, 1), flip);
        }

        private static void SetColor(Vertex[] buffer, int offset, Color c)
        {
            buffer[offset + 0].Tint = c;
            buffer[offset + 1].Tint = c;
            buffer[offset + 2].Tint = c;

            buffer[offset + 3].Tint = c;
            buffer[offset + 4].Tint = c;
            buffer[offset + 5].Tint = c;
        }

        private static FRect MakeVertRect(Point drawsize, Vector2 location, Vector2 offset, Vector2 scale, Vector2 axis, SpriteEffects flip)
        {
            var r = new FRect();

            var vloc = GetDrawLocation(drawsize, location, axis, scale, flip) + offset;
            r.X = vloc.X;
            r.Y = vloc.Y;
            r.Width = drawsize.X * scale.X;
            r.Height = drawsize.Y * scale.Y;

            return r;
        }

        public static Vector2 GetDrawLocation(Point drawsize, Vector2 location, Vector2 axis, Vector2 scale, SpriteEffects flip)
        {
            var drawlocation = location - new Vector2(0.5f, 0.5f);

            if ((flip & SpriteEffects.FlipHorizontally) != 0)
            {
                drawlocation.X -= (drawsize.X - axis.X - 1) * scale.X;
            }
            else
            {
                drawlocation.X -= axis.X * scale.X;
            }

            if ((flip & SpriteEffects.FlipVertically) != 0)
            {
                drawlocation.Y -= (drawsize.Y - axis.Y - 1) * scale.Y;
            }
            else
            {
                drawlocation.Y -= axis.Y * scale.Y;
            }

            return drawlocation;
        }

        public bool UseOldShader
        {
            get => m_useoldshader;

            set { m_useoldshader = value; }
        }

        private Color Tint => m_videosystem.Tint;

        private Point ScreenSize => m_videosystem.ScreenSize;

        private Vector2 ScreenScale => (Vector2)ScreenSize / (Vector2)Mugen.ScreenSize;

        private Vector2 CameraShift => (Vector2)m_videosystem.CameraShift;

        private GraphicsDevice Device => m_videosystem.Device;

        #region	Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly VideoSystem m_videosystem;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Effect m_effect;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vertex[] m_drawbuffer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly KeyedCollection<string, EffectParameter> m_parameters;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Texture2D m_nullpixels;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Texture2D m_nullpalette;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool m_useoldshader;

        #endregion
    }
}