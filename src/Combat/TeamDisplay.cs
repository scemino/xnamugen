using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
    class TeamDisplay
    {
        public TeamDisplay(Team team)
        {
            if (team == null) throw new ArgumentNullException("team");

            m_team = team;
            m_combocounter = new ComboCounter(team);

            IO.TextFile textfile = m_team.Engine.GetSubSystem<IO.FileSystem>().OpenTextFile(@"data/fight.def");
            IO.TextSection lifebar = textfile.GetSection("Lifebar");
            IO.TextSection powerbar = textfile.GetSection("Powerbar");
            IO.TextSection face = textfile.GetSection("Face");
            IO.TextSection name = textfile.GetSection("Name");
            IO.TextSection winicon = textfile.GetSection("WinIcon");

            String prefix = Misc.GetPrefix(m_team.Side);
            var elements = m_team.Engine.Elements;

            m_lifebg0 = elements.Build(prefix + "lifebar.bg0", lifebar, prefix + ".bg0");
            m_lifebg1 = elements.Build(prefix + "lifebar.bg1", lifebar, prefix + ".bg1");
            m_lifebg2 = elements.Build(prefix + "lifebar.bg2", lifebar, prefix + ".bg2");
            m_lifemid = elements.Build(prefix + "lifebar.mid", lifebar, prefix + ".mid");
            m_lifefront = elements.Build(prefix + "lifebar.front", lifebar, prefix + ".front");

            m_powerbg0 = elements.Build(prefix + "powerbar.bg0", powerbar, prefix + ".bg0");
            m_powerbg1 = elements.Build(prefix + "powerbar.bg1", powerbar, prefix + ".bg1");
            m_powerbg2 = elements.Build(prefix + "powerbar.bg2", powerbar, prefix + ".bg2");
            m_powermid = elements.Build(prefix + "powerbar.mid", powerbar, prefix + ".mid");
            m_powerfront = elements.Build(prefix + "powerbar.front", powerbar, prefix + ".front");
            m_powercounter = elements.Build(prefix + "powerbar.counter", powerbar, prefix + ".counter");

            m_facebg = elements.Build(prefix + "face.bg", face, prefix + ".bg");
            m_faceimage = elements.Build(prefix + "face.face", face, prefix + ".face");

            m_namelement = elements.Build(prefix + "name.name", name, prefix + ".name");

            m_winiconnormal = elements.Build(prefix + "winicon.normal", winicon, prefix + ".n");
            m_winiconspecial = elements.Build(prefix + "winicon.special", winicon, prefix + ".s");
            m_winiconhyper = elements.Build(prefix + "winicon.hyper", winicon, prefix + ".h");
            m_winiconthrow = elements.Build(prefix + "winicon.normalthrow", winicon, prefix + ".throw");
            m_winiconcheese = elements.Build(prefix + "winicon.cheese", winicon, prefix + ".c");
            m_winicontime = elements.Build(prefix + "winicon.timeout", winicon, prefix + ".t");
            m_winiconsuicide = elements.Build(prefix + "winicon.suicide", winicon, prefix + ".suicide");
            m_winiconteammate = elements.Build(prefix + "winicon.teammate", winicon, prefix + ".teammate");
            m_winiconperfect = elements.Build(prefix + "winicon.perfect", winicon, prefix + ".perfect");

            m_lifebarposition = (Vector2)lifebar.GetAttribute<Point>(prefix + ".pos");
            m_lifebarrange = lifebar.GetAttribute<Point>(prefix + ".range.x");

            m_powerbarposition = (Vector2)powerbar.GetAttribute<Point>(prefix + ".pos");
            m_powerbarrange = powerbar.GetAttribute<Point>(prefix + ".range.x");

            m_faceposition = (Vector2)face.GetAttribute<Point>(prefix + ".pos");

            m_nameposition = (Vector2)name.GetAttribute<Point>(prefix + ".pos");

            m_winiconposition = (Vector2)winicon.GetAttribute<Point>(prefix + ".pos");
            m_winiconoffset = (Vector2)winicon.GetAttribute<Point>(prefix + ".iconoffset");
        }

        public void Update()
        {
            ComboCounter.Update();
        }

        public void Draw()
        {
            DrawLifebar(m_team.MainPlayer);
            DrawPowerbar(m_team.MainPlayer);
            DrawFace(m_team.MainPlayer);
            PrintName(m_team.MainPlayer);

            DrawWinIcons();
        }

        void DrawWinIcons()
        {
            Vector2 location = m_winiconposition;

            foreach (Win win in m_team.Wins)
            {
                switch (win.Victory)
                {
                    case Victory.Normal:
                        m_winiconnormal.Draw(location);
                        break;

                    case Victory.Special:
                        m_winiconspecial.Draw(location);
                        break;

                    case Victory.Hyper:
                        m_winiconhyper.Draw(location);
                        break;

                    case Victory.NormalThrow:
                        m_winiconthrow.Draw(location);
                        break;

                    case Victory.Cheese:
                        m_winiconcheese.Draw(location);
                        break;

                    case Victory.Time:
                        m_winicontime.Draw(location);
                        break;

                    case Victory.Suicude:
                        m_winiconsuicide.Draw(location);
                        break;

                    case Victory.TeamKill:
                        m_winiconteammate.Draw(location);
                        break;
                }

                if (win.IsPerfectVictory == true) m_winiconperfect.Draw(location);

                location += m_winiconoffset;
            }
        }

        void DrawLifebar(Player player)
        {
            if (player == null) throw new ArgumentNullException("player");

            if (m_lifebg0.DataMap.Type == ElementType.Static || m_lifebg0.DataMap.Type == ElementType.Animation)
            {
                m_lifebg0.Draw(m_lifebarposition);
            }

            if (m_lifebg1.DataMap.Type == ElementType.Static || m_lifebg1.DataMap.Type == ElementType.Animation)
            {
                m_lifebg1.Draw(m_lifebarposition);
            }

            if (m_lifemid.DataMap.Type == ElementType.Static)
            {
                //m_lifemid.Draw(m_lifebarposition);
            }

            if (m_lifefront.DataMap.Type == ElementType.Static)
            {
                Single life_percentage = Math.Max(0.0f, (Single)player.Life / (Single)player.Constants.MaximumLife);

                var drawstate = m_lifefront.SpriteManager.SetupDrawing(m_lifefront.DataMap.SpriteId, m_lifebarposition, Vector2.Zero, m_lifefront.DataMap.Scale, m_lifefront.DataMap.Flip);
                drawstate.ScissorRectangle = CreateBarScissorRectangle(m_lifefront, m_lifebarposition, life_percentage, m_lifebarrange);
                drawstate.Use();
            }
        }

        void DrawPowerbar(Player player)
        {
            if (player == null) throw new ArgumentNullException("player");

            if (m_powerbg0.DataMap.Type == ElementType.Static || m_powerbg0.DataMap.Type == ElementType.Animation)
            {
                m_powerbg0.Draw(m_powerbarposition);
            }

            if (m_powerbg1.DataMap.Type == ElementType.Static || m_powerbg1.DataMap.Type == ElementType.Animation)
            {
                m_powerbg1.Draw(m_powerbarposition);
            }

            if (m_powermid.DataMap.Type == ElementType.Static || m_powermid.DataMap.Type == ElementType.Animation)
            {
                //m_powermid.Draw(m_powerbarposition);
            }

            if (m_powerfront != null && m_powerfront.DataMap.Type == ElementType.Static)
            {
                Single power_percentage = Math.Max(0.0f, (Single)player.Power / (Single)player.Constants.MaximumPower);

                var drawstate = m_powerfront.SpriteManager.SetupDrawing(m_powerfront.DataMap.SpriteId, m_powerbarposition, Vector2.Zero, m_powerfront.DataMap.Scale, m_powerfront.DataMap.Flip);
                drawstate.ScissorRectangle = CreateBarScissorRectangle(m_powerfront, m_powerbarposition, power_percentage, m_powerbarrange);
                drawstate.Use();
            }

			if (m_powercounter.DataMap.Type == ElementType.Text)
			{
				String powertext = (player.Power / 1000).ToString();
				m_team.Engine.Print(m_powercounter.DataMap.FontData, m_powerbarposition + m_powercounter.DataMap.Offset, powertext, null);
			}
        }

        void DrawFace(Player player)
        {
            if (player == null) throw new ArgumentNullException("player");

            if (m_facebg.DataMap.Type == ElementType.Static || m_facebg.DataMap.Type == ElementType.Animation)
            {
                m_facebg.Draw(m_faceposition);
            }

            if (m_faceimage != null && m_faceimage.DataMap.Type == ElementType.Static)
            {
                var drawstate = player.SpriteManager.SetupDrawing(m_faceimage.DataMap.SpriteId, m_faceposition + m_faceimage.DataMap.Offset, Vector2.Zero, m_faceimage.DataMap.Scale, m_faceimage.DataMap.Flip);

                player.PaletteFx.SetShader(drawstate.ShaderParameters);

                drawstate.Use();
            }
        }

        void PrintName(Player player)
        {
            if (player == null) throw new ArgumentNullException("player");

            if (m_namelement.DataMap.Type == ElementType.Text)
            {
                m_team.Engine.Print(m_namelement.DataMap.FontData, m_nameposition, player.Profile.DisplayName, null);
            }
        }

        Rectangle CreateBarScissorRectangle(Elements.Base element, Vector2 location, Single percentage, Point range)
        {
            if (element == null) throw new ArgumentNullException("element");

            Drawing.Sprite sprite = element.SpriteManager.GetSprite(element.DataMap.SpriteId);
            if (sprite == null) return new Rectangle();

            Point drawlocation = (Point)Video.Renderer.GetDrawLocation(sprite.Size, location, (Vector2)sprite.Axis, element.DataMap.Scale, Vector2.One, element.DataMap.Flip);

            Rectangle rectangle = new Rectangle();
            rectangle.X = (Int32)element.DataMap.Offset.X + drawlocation.X + 1;
            rectangle.Y = (Int32)element.DataMap.Offset.Y + drawlocation.Y;
            rectangle.Height = sprite.Size.Y + 1;
            rectangle.Width = sprite.Size.X + 1;

            Int32 position = (Int32)MathHelper.Lerp(range.X, range.Y, percentage);
            if (position > 0)
            {
                rectangle.Width = position + 2;
            }
            else if (position < 0)
            {
                rectangle.Width = -position + 2;

                rectangle.X += position - range.Y - 1;
            }
            else
            {
                rectangle.Width = 0;
            }

            return rectangle;
        }

        public ComboCounter ComboCounter
        {
            get { return m_combocounter; }
        }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Team m_team;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_lifebarposition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Point m_lifebarrange;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_powerbarposition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Point m_powerbarrange;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_faceposition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_nameposition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_winiconposition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Vector2 m_winiconoffset;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly ComboCounter m_combocounter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_lifebg0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_lifebg1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_lifebg2;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_lifemid;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_lifefront;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_powerbg0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_powerbg1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_powerbg2;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_powermid;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_powerfront;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_powercounter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_facebg;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_faceimage;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_namelement;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_winiconnormal;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_winiconspecial;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_winiconhyper;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_winiconthrow;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_winiconcheese;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_winicontime;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_winiconsuicide;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_winiconteammate;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Base m_winiconperfect;

        #endregion
    }
}