using System;
using Microsoft.Xna.Framework;
using xnaMugen.Elements;
using xnaMugen.Video;

namespace xnaMugen.Combat
{
    internal class Lifebar
    {
        private const int TotalDamageWait = 30;

        private int m_currentLife;
        private int m_damageWait;
        private int m_damage;
        private readonly Base m_lifebg0;
        private readonly Base m_lifebg1;
        private readonly Base m_lifeMid;
        private readonly Vector2 m_lifebarposition;
        private readonly Base m_lifeFront;
        private readonly Point m_lifebarRange;

        public Lifebar(Base lifebg0, Base lifebg1, Base lifeMid, Base lifeFront, Vector2 lifebarposition,
            Point lifebarrange)
        {
            m_currentLife = 1000;
            m_damage = 1000;
            m_lifebg0 = lifebg0;
            m_lifebg1 = lifebg1;
            m_lifeMid = lifeMid;
            m_lifeFront = lifeFront;
            m_lifebarposition = lifebarposition;
            m_lifebarRange = lifebarrange;
        }

        public void Draw(Player player)
        {
            if (m_lifebg0.DataMap.Type == ElementType.Static || m_lifebg0.DataMap.Type == ElementType.Animation)
            {
                m_lifebg0.Draw(m_lifebarposition);
            }

            if (m_lifebg1.DataMap.Type == ElementType.Static || m_lifebg1.DataMap.Type == ElementType.Animation)
            {
                m_lifebg1.Draw(m_lifebarposition);
            }

            if (m_lifeMid.DataMap.Type == ElementType.Static)
            {
                var lifePercentage = m_damage / (float) player.Constants.MaximumLife;

                var drawstate = m_lifeMid.SpriteManager.SetupDrawing(m_lifeMid.DataMap.SpriteId, m_lifebarposition,
                    Vector2.Zero, m_lifeMid.DataMap.Scale, m_lifeMid.DataMap.Flip);
                drawstate.ScissorRectangle =
                    DrawState.CreateBarScissorRectangle(m_lifeMid, m_lifebarposition, lifePercentage, m_lifebarRange);
                drawstate.Use();
            }

            if (m_lifeFront.DataMap.Type == ElementType.Static)
            {
                var lifePercentage = Math.Max(0.0f, player.Life / (float) player.Constants.MaximumLife);

                var drawstate = m_lifeFront.SpriteManager.SetupDrawing(m_lifeFront.DataMap.SpriteId, m_lifebarposition,
                    Vector2.Zero, m_lifeFront.DataMap.Scale, m_lifeFront.DataMap.Flip);
                drawstate.ScissorRectangle =
                    DrawState.CreateBarScissorRectangle(m_lifeFront, m_lifebarposition, lifePercentage, m_lifebarRange);
                drawstate.Use();
            }
        }

        public void Update(Player player)
        {
            if (m_currentLife != player.Life)
            {
                m_damageWait = TotalDamageWait;
            }

            m_currentLife = player.Life;
            if (m_damage == m_currentLife)
            {
                m_damageWait = TotalDamageWait;
            }
            else
            {
                if (m_damageWait <= 0)
                {
                    m_damage -= (int) ((m_damage - m_currentLife) / 6.0 + 0.5);
                }
                else
                {
                    m_damageWait--;
                }
            }
        }
    }
}