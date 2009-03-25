using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.Collections;
using System.Reflection;

namespace xnaMugen.Video
{
	class Renderer : Resource
	{
		public Renderer(VideoSystem videosystem)
		{
			if (videosystem == null) throw new ArgumentNullException("videosystem");

			m_videosystem = videosystem;

			using (IO.File effectfile = m_videosystem.GetSubSystem<IO.FileSystem>().OpenFile("xnaMugen.data.Shader.fx"))
			{
				CompiledEffect compiled = Effect.CompileEffectFromSource(effectfile.ReadToEnd(), null, null, CompilerOptions.None, TargetPlatform.Windows);
				if (compiled.Success == false)
				{
					throw new InvalidOperationException("Cannot successfully create shader.");
				}

				m_effect = new Effect(Device, compiled.GetEffectCode(), CompilerOptions.NotCloneable, null);
			}

			m_drawbuffer = new Vertex[500];
			m_parameters = new KeyedCollection<String, EffectParameter>(x => x.Name);

			m_nullpixels = m_videosystem.CreatePixelTexture(new Point(1, 1));
			m_nullpalette = m_videosystem.CreatePaletteTexture();

			Byte[] pixels = new Byte[] { 1 };
			m_nullpixels.SetData<Byte>(pixels);

			Color[] paldata = new Color[256];
			paldata[1] = Color.White;
			m_nullpalette.SetData<Color>(paldata);
		}

		public void OnDeviceReset(Object sender, EventArgs args)
		{
			Matrix m = Matrix.Identity;
			m *= Matrix.CreateScale(1, -1, -1);
			m *= Matrix.CreateScale(ScreenScale.X, ScreenScale.Y, 1);
			m *= Matrix.CreateOrthographicOffCenter(0, ScreenSize.X, -ScreenSize.Y, 0, 0, 1);

			GetShaderParameter("xMatrix").SetValue(m);
			GetShaderParameter("xRotation").SetValue(Matrix.Identity);
			GetShaderParameter("xBlendWeight").SetValue(1.0f);

			Device.RenderState.AlphaBlendEnable = false;
		}

		public void Draw(DrawState drawstate)
		{
			if (drawstate == null) throw new ArgumentNullException("drawstate");

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

		void SetBlending(Blending blending)
		{
			RenderState r = Device.RenderState;

			switch (blending.BlendType)
			{
				case BlendType.Add:
					GetShaderParameter("xBlendWeight").SetValue((Single)blending.SourceFactor / 255.0f);

					r.AlphaBlendEnable = true;
					r.BlendFunction = BlendFunction.Add;
					r.SourceBlend = Blend.SourceAlpha;
					r.DestinationBlend = Blend.BlendFactor;
					r.BlendFactor = new Color(blending.DestinationFactor, blending.DestinationFactor, blending.DestinationFactor, 255);
					break;

				case BlendType.Subtract:
					GetShaderParameter("xBlendWeight").SetValue((Single)blending.SourceFactor / 255.0f);

					r.AlphaBlendEnable = true;
					r.BlendFunction = BlendFunction.ReverseSubtract;
					r.SourceBlend = Blend.SourceAlpha;
					r.DestinationBlend = Blend.BlendFactor;
					r.BlendFactor = new Color(blending.DestinationFactor, blending.DestinationFactor, blending.DestinationFactor, 255);
					break;

				default:
				case BlendType.None:
					GetShaderParameter("xBlendWeight").SetValue(1.0f);
					r.AlphaBlendEnable = false;
					break;
			}
		}

		void SetScissorTest(Rectangle rectangle)
		{
			if (rectangle.IsEmpty == true)
			{
				Device.RenderState.ScissorTestEnable = false;
			}
			else
			{
				rectangle.X = (Int32)(rectangle.X * ScreenScale.X);
				rectangle.Width = (Int32)(rectangle.Width * ScreenScale.X);

				rectangle.Y = (Int32)(rectangle.Y * ScreenScale.Y);
				rectangle.Height = (Int32)(rectangle.Height * ScreenScale.Y);

				Device.RenderState.ScissorTestEnable = true;
				Device.ScissorRectangle = rectangle;
			}
		}

		Int32 DefaultDrawSetup(DrawState drawstate, Point pixelsize)
		{
			if (drawstate == null) throw new ArgumentNullException("drawstate");

			Int32 count = 0;
			foreach (DrawData data in drawstate)
			{
				Point drawsize = new Point();

				if (data.DrawRect != null)
				{
					drawsize = new Point(data.DrawRect.Value.Width, data.DrawRect.Value.Height);

					Renderer.SetTextureCoords(m_drawbuffer, count * 6, pixelsize, data.DrawRect.Value, drawstate.Flip);
				}
				else
				{
					drawsize = pixelsize;

					Renderer.SetTextureCoords(m_drawbuffer, count * 6, drawstate.Flip);
				}

				FRect frect = Renderer.MakeVertRect(drawsize, data.Location, CameraShift, drawstate.Scale, drawstate.Axis - drawstate.Offset, drawstate.Flip);

				Vector2 rotationaxis = data.Location + CameraShift + drawstate.Offset;

				Renderer.SetPosition(m_drawbuffer, count * 6, frect, MathHelper.ToRadians(drawstate.Rotation), rotationaxis);
				Renderer.SetColor(m_drawbuffer, count * 6, Misc.BlendColors(Tint, data.Tint));

				++count;
			}

			return count;
		}

		void NormalDraw(DrawState drawstate)
		{
			if (drawstate == null) throw new ArgumentNullException("drawstate");
			if (drawstate.Mode != DrawMode.Normal) throw new ArgumentException("Incorrect drawstate");

			m_effect.CurrentTechnique = (UseOldShader == true) ? m_effect.Techniques["DrawOLD"] : m_effect.Techniques["Draw"];

			SetShaderParameters(drawstate.ShaderParameters, drawstate.Pixels, drawstate.Palette);

			Int32 count = DefaultDrawSetup(drawstate, new Point(drawstate.Pixels.Width, drawstate.Pixels.Height));
			if (count > 0) FinishDrawing(PrimitiveType.TriangleList, count * 2);
		}

		void FontDraw(DrawState drawstate)
		{
			if (drawstate == null) throw new ArgumentNullException("drawstate");
			if (drawstate.Mode != DrawMode.Font) throw new ArgumentException("Incorrect drawstate");

			m_effect.CurrentTechnique = m_effect.Techniques["FontDraw"];

			SetShaderParameters(drawstate.ShaderParameters, drawstate.Pixels, drawstate.Palette);

			Int32 count = DefaultDrawSetup(drawstate, new Point(drawstate.Pixels.Width, drawstate.Pixels.Height));
			if (count > 0) FinishDrawing(PrimitiveType.TriangleList, count * 2);
		}

		void OutlinedRectangleDraw(DrawState drawstate)
		{
			if (drawstate == null) throw new ArgumentNullException("drawstate");
			if (drawstate.Mode != DrawMode.OutlinedRectangle) throw new ArgumentException("Incorrect drawstate");

			m_effect.CurrentTechnique = (UseOldShader == true) ? m_effect.Techniques["DrawOLD"] : m_effect.Techniques["Draw"];

			SetShaderParameters(drawstate.ShaderParameters, drawstate.Pixels, drawstate.Palette);

			Int32 count = 0;
			foreach (DrawData data in drawstate)
			{
				if (data.DrawRect != null)
				{
					Point drawsize = new Point(data.DrawRect.Value.Width, data.DrawRect.Value.Height);
					FRect frect = Renderer.MakeVertRect(drawsize, data.Location, CameraShift, drawstate.Scale, drawstate.Axis - drawstate.Offset, drawstate.Flip);

					m_drawbuffer[(count * 8) + 0].Position = new Vector2(frect.Left, frect.Top);
					m_drawbuffer[(count * 8) + 1].Position = new Vector2(frect.Left, frect.Bottom);
					m_drawbuffer[(count * 8) + 2].Position = new Vector2(frect.Right, frect.Top);
					m_drawbuffer[(count * 8) + 3].Position = new Vector2(frect.Right, frect.Bottom);
					m_drawbuffer[(count * 8) + 4].Position = new Vector2(frect.Left, frect.Top);
					m_drawbuffer[(count * 8) + 5].Position = new Vector2(frect.Right, frect.Top);
					m_drawbuffer[(count * 8) + 6].Position = new Vector2(frect.Left, frect.Bottom);
					m_drawbuffer[(count * 8) + 7].Position = new Vector2(frect.Right, frect.Bottom);

					for (Int32 i = 0; i != 8; ++i) m_drawbuffer[(count * 8) + i].Tint = data.Tint;

					++count;
				}
			}

			if (count > 0) FinishDrawing(PrimitiveType.LineList, count * 4);
		}

		void FilledRectangleDraw(DrawState drawstate)
		{
			if (drawstate == null) throw new ArgumentNullException("drawstate");
			if (drawstate.Mode != DrawMode.FilledRectangle) throw new ArgumentException("Incorrect drawstate");

			m_effect.CurrentTechnique = (UseOldShader == true) ? m_effect.Techniques["DrawOLD"] : m_effect.Techniques["Draw"];

			SetShaderParameters(drawstate.ShaderParameters, drawstate.Pixels, drawstate.Palette);

			Int32 count = DefaultDrawSetup(drawstate, new Point(drawstate.Pixels.Width, drawstate.Pixels.Height));
			if (count > 0) FinishDrawing(PrimitiveType.TriangleList, count * 2);
		}

		void LineDraw(DrawState drawstate)
		{
			if (drawstate == null) throw new ArgumentNullException("drawstate");
			if (drawstate.Mode != DrawMode.Lines) throw new ArgumentException("Incorrect drawstate");

			m_effect.CurrentTechnique = (UseOldShader == true) ? m_effect.Techniques["DrawOLD"] : m_effect.Techniques["Draw"];

			SetShaderParameters(drawstate.ShaderParameters, drawstate.Pixels, drawstate.Palette);

			Int32 count = 0;
			Boolean point = false;
			foreach (DrawData data in drawstate)
			{
				if (point == false)
				{
					point = true;

					m_drawbuffer[count * 2 + 0] = new Vertex(data.Location + CameraShift, Vector2.Zero, data.Tint);
				}
				else
				{
					point = false;

					m_drawbuffer[count * 2 + 1] = new Vertex(data.Location + CameraShift, Vector2.Zero, data.Tint);
					++count;
				}
			}

			if (count > 0) FinishDrawing(PrimitiveType.LineList, count);
		}

		void FinishDrawing(PrimitiveType drawtype, Int32 count)
		{
			m_effect.Begin();
			for (Int32 i = 0; i != m_effect.CurrentTechnique.Passes.Count; ++i)
			{
				EffectPass pass = m_effect.CurrentTechnique.Passes[i];

				pass.Begin();
				Device.DrawUserPrimitives<Vertex>(drawtype, m_drawbuffer, 0, count);
				pass.End();
			}
			m_effect.End();

			m_effect.GraphicsDevice.Textures[0] = null;
			m_effect.GraphicsDevice.Textures[1] = null;
		}

		void SetShaderParameters(ShaderParameters parameters, Texture2D pixels, Texture2D palette)
		{
			if (parameters == null) throw new ArgumentNullException("parameters");
			if (pixels == null) throw new ArgumentNullException("pixels");
			if (palette == null) throw new ArgumentNullException("palette");

			GetShaderParameter("xPixels").SetValue(pixels);
			GetShaderParameter("xPalette").SetValue(palette);

			GetShaderParameter("xFontColorIndex").SetValue(parameters.FontColorIndex);
			GetShaderParameter("xFontTotalColors").SetValue(parameters.FontTotalColors);

			if (parameters.PaletteFxEnable == true)
			{
				GetShaderParameter("xPalFx_Use").SetValue(true);
				GetShaderParameter("xPalFx_Add").SetValue(new Vector4(parameters.PaletteFxAdd, 0));
				GetShaderParameter("xPalFx_Mul").SetValue(new Vector4(parameters.PaletteFxMultiply, 1));
				GetShaderParameter("xPalFx_Invert").SetValue(parameters.PaletteFxInvert);
				GetShaderParameter("xPalFx_Color").SetValue(parameters.PaletteFxColor);

				Vector4 sincolor = parameters.PaletteFxSinAdd * (Single)Math.Sin(parameters.PaletteFxTime * MathHelper.TwoPi / parameters.PaletteFxSinAdd.W);
				sincolor.W = 0;

				GetShaderParameter("xPalFx_SinMath").SetValue(sincolor);
			}
			else
			{
				GetShaderParameter("xPalFx_Use").SetValue(false);
			}

			if (parameters.AfterImageEnable == true)
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

		EffectParameter GetShaderParameter(String name)
		{
			if (name == null) throw new ArgumentNullException("name");

			if (m_parameters.Contains(name) == true) return m_parameters[name];

			EffectParameter parameter = m_effect.Parameters[name];
			m_parameters.Add(parameter);

			return parameter;
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_nullpixels != null) m_nullpixels.Dispose();

				if (m_nullpalette != null) m_nullpalette.Dispose();
			}

			base.Dispose(disposing);
		}

		static String CreateShaderCode()
		{
			var sb = new System.Text.StringBuilder();

			sb.AppendLine(@"float PI = 3.14159f;");

			sb.AppendLine(@"Texture xPixels;");
			sb.AppendLine(@"sampler xPixelsSampler = sampler_state { texture = <xPixels>; AddressU = clamp; AddressV = clamp; magfilter = point; minfilter = point; mipfilter = point; };");

			sb.AppendLine(@"Texture xPalette;");
			sb.AppendLine(@"sampler xPaletteSampler = sampler_state { texture = <xPalette>; AddressU = clamp; AddressV = clamp; magfilter = none; minfilter = none; mipfilter = none; };");

			sb.AppendLine(@"float4x4 xMatrix;");
			sb.AppendLine(@"float4x4 xRotation;");

			sb.AppendLine(@"int xFontColorIndex;");
			sb.AppendLine(@"int xFontTotalColors;");

			sb.AppendLine(@"bool xPalFx_Use;");
			sb.AppendLine(@"int xPalFx_Time;");
			sb.AppendLine(@"float3 xPalFx_Add;");
			sb.AppendLine(@"float3 xPalFx_Mul;");
			sb.AppendLine(@"float4 xPalFx_SinAdd;");
			sb.AppendLine(@"bool xPalFx_Invert;");
			sb.AppendLine(@"float3 xPalFx_Color;");

			sb.AppendLine(@"bool xAI_Use;");
			sb.AppendLine(@"bool xAI_Invert;");
			sb.AppendLine(@"float3 xAI_color;");
			sb.AppendLine(@"float3 xAI_preadd;");
			sb.AppendLine(@"float3 xAI_contrast;");
			sb.AppendLine(@"float3 xAI_postadd;");
			sb.AppendLine(@"float3 xAI_paladd;");
			sb.AppendLine(@"float3 xAI_palmul;");
			sb.AppendLine(@"int xAI_number;");

			sb.AppendLine(@"float4 PalFx(float4 inputcolor)");
			sb.AppendLine(@"{");
			sb.AppendLine(@"float4 output = inputcolor;");
			sb.AppendLine(@"output *= float4(xPalFx_Color, 1);");
			sb.AppendLine(@"if(xPalFx_Invert == true) output = float4(1 - output.r, 1 - output.g, 1 - output.b, output.a);");
			sb.AppendLine(@"float4 sincolor = float4(xPalFx_SinAdd.rgb, 0) * sin(xPalFx_Time * 2 * PI / xPalFx_SinAdd.a);");
			sb.AppendLine(@"output = (output + float4(xPalFx_Add, 0) + sincolor) * float4(xPalFx_Mul, 1);");
			sb.AppendLine(@"return output;");
			sb.AppendLine(@"}");

			sb.AppendLine(@"float4 AfterImage(float4 inputcolor)");
			sb.AppendLine(@"{");
			sb.AppendLine(@"float4 output = inputcolor;");
			sb.AppendLine(@"output *= float4(xAI_color, 1);");
			sb.AppendLine(@"if(xAI_Invert == true) output = float4(1, 1, 1, 1) - output;");
			sb.AppendLine(@"output += float4(xAI_preadd, 0);");
			sb.AppendLine(@"output *= float4(xAI_contrast, 1);");
			sb.AppendLine(@"output += float4(xAI_postadd, 0);");
			sb.AppendLine(@"for(int i = 0; i < xAI_number; ++i)");
			sb.AppendLine(@"{");
			sb.AppendLine(@"output += float4(xAI_paladd, 0);");
			sb.AppendLine(@"output *= float4(xAI_palmul, 1);");
			sb.AppendLine(@"}");
			sb.AppendLine(@"return output;");
			sb.AppendLine(@"}");

			sb.AppendLine(@"struct VS_Output");
			sb.AppendLine(@"{");
			sb.AppendLine(@"float4 Position : POSITION;");
			sb.AppendLine(@"float2 TextureCoords: TEXCOORD;");
			sb.AppendLine(@"float4 Color: COLOR;");
			sb.AppendLine(@"};");

			sb.AppendLine(@"VS_Output VertexShader(float4 inPos : POSITION, float2 inTexCoords: TEXCOORD, float4 inColor : COLOR)");
			sb.AppendLine(@"{");
			sb.AppendLine(@"VS_Output Output = (VS_Output)0;");
			sb.AppendLine(@"Output.Position = mul(inPos, xMatrix);");
			sb.AppendLine(@"Output.Position = mul(Output.Position, xRotation);");
			sb.AppendLine(@"Output.TextureCoords = inTexCoords;");
			sb.AppendLine(@"Output.Color = inColor;");
			sb.AppendLine(@"return Output;");
			sb.AppendLine(@"}");

			sb.AppendLine(@"float4 DefaultPixelShader(float4 color : COLOR, float2 texCoord : TEXCOORD) : COLOR");
			sb.AppendLine(@"{");
			sb.AppendLine(@"float color_index = tex2D(xPixelsSampler, texCoord).a;");
			sb.AppendLine(@"float4 output_color = tex1D(xPaletteSampler, color_index);");
			sb.AppendLine(@"if(xPalFx_Use == true) output_color = PalFx(output_color);");
			sb.AppendLine(@"if(xAI_Use == true) output_color = AfterImage(output_color);");
			sb.AppendLine(@"output_color *= color;");
			sb.AppendLine(@"return output_color;");
			sb.AppendLine(@"}");

			sb.AppendLine(@"float4 FontPixelShader(float4 color : COLOR, float2 texCoord : TEXCOORD) : COLOR");
			sb.AppendLine(@"{");
			sb.AppendLine(@"float color_index = tex2D(xPixelsSampler, texCoord).a;");
			sb.AppendLine(@"float per = 1.0 - float(xFontColorIndex) / float(xFontTotalColors);");
			sb.AppendLine(@"float4 output_color = tex1D(xPaletteSampler, color_index * per - 1.0/255.0);");
			sb.AppendLine(@"if(xPalFx_Use == true) output_color = PalFx(output_color);");
			sb.AppendLine(@"if(xAI_Use == true) output_color = AfterImage(output_color);");
			sb.AppendLine(@"output_color *= color;");
			sb.AppendLine(@"return output_color;");
			sb.AppendLine(@"}");

			sb.AppendLine(@"technique Draw");
			sb.AppendLine(@"{");
			sb.AppendLine(@"pass Pass0");
			sb.AppendLine(@"{");
			sb.AppendLine(@"VertexShader = compile vs_1_1 VertexShader();");
			sb.AppendLine(@"PixelShader = compile ps_3_0 DefaultPixelShader();");
			sb.AppendLine(@"}");
			sb.AppendLine(@"}");

			sb.AppendLine(@"technique FontDraw");
			sb.AppendLine(@"{");
			sb.AppendLine(@"pass Pass0");
			sb.AppendLine(@"{");
			sb.AppendLine(@"VertexShader = compile vs_1_1 VertexShader();");
			sb.AppendLine(@"PixelShader = compile ps_3_0 FontPixelShader();");
			sb.AppendLine(@"}");
			sb.AppendLine(@"}");

			return sb.ToString();
		}

		static void SetPosition(Vertex[] buffer, Int32 offset, FRect r, Single rotation, Vector2 axis)
		{
			Vector2 p1 = new Vector2(r.Left, r.Top);
			Vector2 p2 = new Vector2(r.Right, r.Top);
			Vector2 p3 = new Vector2(r.Right, r.Bottom);
			Vector2 p4 = new Vector2(r.Left, r.Bottom);

			p1 = RotatePoint(p1, rotation, axis);
			p2 = RotatePoint(p2, rotation, axis);
			p3 = RotatePoint(p3, rotation, axis);
			p4 = RotatePoint(p4, rotation, axis);

			buffer[offset + 0].Position = p1;
			buffer[offset + 1].Position = p2;
			buffer[offset + 2].Position = p3;

			buffer[offset + 3].Position = p1;
			buffer[offset + 4].Position = p3;
			buffer[offset + 5].Position = p4;
		}

		static Vector2 RotatePoint(Vector2 point, Single radians, Vector2 axis)
		{
			Double x = Math.Cos(radians) * (point.X - axis.X) - Math.Sin(radians) * (point.Y - axis.Y);
			Double y = Math.Sin(radians) * (point.X - axis.X) + Math.Cos(radians) * (point.Y - axis.Y);

			return new Vector2((Single)x + axis.X, (Single)y + axis.Y);
		}

		static void SetTextureCoords(Vertex[] buffer, Int32 offset, Point textsize, Rectangle textrect, SpriteEffects flip)
		{
			Single x1 = (Single)textrect.Left / (Single)textsize.X;
			Single x2 = (Single)textrect.Right / (Single)textsize.X;
			Single y1 = (Single)textrect.Top / (Single)textsize.Y;
			Single y2 = (Single)textrect.Bottom / (Single)textsize.Y;

			if ((flip & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally)
			{
				Single temp = x1;
				x1 = x2;
				x2 = temp;
			}

			if ((flip & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically)
			{
				Single temp = y1;
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

		static void SetTextureCoords(Vertex[] buffer, Int32 offset, SpriteEffects flip)
		{
			SetTextureCoords(buffer, offset, new Point(1, 1), new Rectangle(0, 0, 1, 1), flip);
		}

		static void SetColor(Vertex[] buffer, Int32 offset, Color c)
		{
			buffer[offset + 0].Tint = c;
			buffer[offset + 1].Tint = c;
			buffer[offset + 2].Tint = c;

			buffer[offset + 3].Tint = c;
			buffer[offset + 4].Tint = c;
			buffer[offset + 5].Tint = c;
		}

		static FRect MakeVertRect(Point drawsize, Vector2 location, Vector2 offset, Vector2 scale, Vector2 axis, SpriteEffects flip)
		{
			FRect r = new FRect();

			Vector2 vloc = GetDrawLocation(drawsize, location, axis, scale, flip) + offset;
			r.X = vloc.X;
			r.Y = vloc.Y;
			r.Width = drawsize.X * scale.X;
			r.Height = drawsize.Y * scale.Y;

			return r;
		}

		public static Vector2 GetDrawLocation(Point drawsize, Vector2 location, Vector2 axis, Vector2 scale, SpriteEffects flip)
		{
			Vector2 drawlocation = location - new Vector2(0.5f, 0.5f);

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

		public Boolean UseOldShader
		{
			get { return m_useoldshader; }

			set { m_useoldshader = value; }
		}

		Color Tint
		{
			get { return m_videosystem.Tint; }
		}

		Point ScreenSize
		{
			get { return m_videosystem.ScreenSize; }
		}

		Vector2 ScreenScale
		{
			get { return (Vector2)ScreenSize / (Vector2)Mugen.ScreenSize; }
		}

		Vector2 CameraShift
		{
			get { return (Vector2)m_videosystem.CameraShift; }
		}

		GraphicsDevice Device
		{
			get { return m_videosystem.Device; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly VideoSystem m_videosystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Effect m_effect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vertex[] m_drawbuffer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly KeyedCollection<String, EffectParameter> m_parameters;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Texture2D m_nullpixels;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Texture2D m_nullpalette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_useoldshader;

		#endregion
	}
}