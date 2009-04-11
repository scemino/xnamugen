using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen
{
	class MugenTest : Game
	{
		public MugenTest()
		{
			IsFixedTimeStep = true;
			TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);

			GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = Mugen.ScreenSize.X * 3;
			graphics.PreferredBackBufferHeight = Mugen.ScreenSize.Y * 3;
			graphics.ApplyChanges();
		}

		protected override void Initialize()
		{
			m_subsystems = new SubSystems(this);
			m_subsystems.GetSubSystem<IO.FileSystem>().Initialize();
			m_subsystems.GetSubSystem<InitializationSettings>().Initialize();
			m_subsystems.GetSubSystem<Input.InputSystem>().Initialize();
			m_subsystems.GetSubSystem<Video.VideoSystem>().Initialize();

			m_font = m_subsystems.GetSubSystem<Drawing.SpriteSystem>().LoadFont(@"font/num1.fnt");
            m_sprites = m_subsystems.GetSubSystem<Drawing.SpriteSystem>().CreateManager(@"chars/BuraiYamamoto/Burai.sff");
            m_animations = m_subsystems.GetSubSystem<Animations.AnimationSystem>().CreateManager(@"chars/BuraiYamamoto/Burai.air");

			//m_sprites = m_subsystems.GetSubSystem<Drawing.SpriteSystem>().CreateManager(@"chars/kfm/kfm.sff");
			//m_animations = m_subsystems.GetSubSystem<Animations.AnimationSystem>().CreateManager(@"chars/kfm/kfm.air");

            m_animations.SetLocalAnimation(25100, 0);
			m_sprites.LoadSprites(m_animations.CurrentAnimation);

			m_subsystems.GetSubSystem<Input.InputSystem>().CurrentInput[0].Add(SystemButton.DebugDraw, this.Click);

			base.Initialize();
		}

		protected override void Update(GameTime gameTime)
		{
			m_subsystems.GetSubSystem<Input.InputSystem>().Update();
			m_animations.Update();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			m_subsystems.GetSubSystem<Video.VideoSystem>().ClearScreen(Color.CornflowerBlue);

			Animations.AnimationElement currentelement = m_animations.CurrentElement;
			Vector2 location = (Vector2)Mugen.ScreenSize / 2;

			DrawElement(location, currentelement);
		}

		void DrawElement(Vector2 location, Animations.AnimationElement element)
		{
			if (element == null) throw new ArgumentNullException("element");

			Vector2 scale = new Vector2(1, 1);

			Video.DrawState drawstate = m_sprites.SetupDrawing(element.SpriteId, location, element.Offset, scale, SpriteEffects.None);
			drawstate.Blending = element.Blending;
			drawstate.Use();

			Drawing.Sprite sprite = m_sprites.GetSprite(element.SpriteId);
			if (sprite != null)
			{
				Vector2 spritelocation = Video.Renderer.GetDrawLocation(sprite.Size, location, (Vector2)sprite.Axis - element.Offset, scale, SpriteEffects.None);
				drawstate.Reset();
				drawstate.Mode = DrawMode.OutlinedRectangle;
				drawstate.AddData(spritelocation, new Rectangle(0, 0, sprite.Size.X, sprite.Size.Y), Color.Gray);
				drawstate.Scale = scale;
				drawstate.Use();
			}


			drawstate.Reset();
			drawstate.Mode = DrawMode.OutlinedRectangle;

			foreach (Animations.Clsn clsn in element)
			{
				Rectangle rect = clsn.MakeRect(location, scale, Facing.Right);

				switch (clsn.ClsnType)
				{
					case ClsnType.Type1Attack:
						drawstate.AddData(new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.Red);
						break;

					case ClsnType.Type2Normal:
						drawstate.AddData(new Vector2(rect.X, rect.Y), new Rectangle(0, 0, rect.Width, rect.Height), Color.Blue);
						break;
				}
			}

			drawstate.Use();


			Int32 line_length = 3;
			drawstate.Reset();
			drawstate.Mode = DrawMode.Lines;
			drawstate.AddData(location - new Vector2(line_length, 0), null, Color.Black);
			drawstate.AddData(location + new Vector2(line_length, 0), null, Color.Black);
			drawstate.AddData(location - new Vector2(0, line_length), null, Color.Black);
			drawstate.AddData(location + new Vector2(0, line_length), null, Color.Black);
			drawstate.Use();
		}

		void Click(Boolean pressed)
		{
			if (pressed == true)
			{
				//m_animations.Update();
				//m_animations.SetLocalAnimation(m_animations.CurrentAnimation.Number, 0);
			}
		}

		SubSystems m_subsystems;
		Drawing.SpriteManager m_sprites;
		Animations.AnimationManager m_animations;
		Drawing.Font m_font;
	}
}