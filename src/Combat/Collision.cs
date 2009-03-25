using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;

namespace xnaMugen.Combat
{
	struct Collision
	{
		public Collision(Entity lhs, ClsnType lhstype, Entity rhs, ClsnType rhstype)
		{
			if (lhs == null) throw new ArgumentNullException("lhs");
			if (rhs == null) throw new ArgumentNullException("rhs");

			m_lhs = lhs;
			m_lhsclsntype = lhstype;
			m_rhs = rhs;
			m_rhsclsntype = rhstype;
			m_type = CollisionType.None;

			if (Left is Player && LeftClsnType == ClsnType.Type2Normal && Right is Player && RightClsnType == ClsnType.Type2Normal)
			{
				m_type = CollisionType.PlayerPush;
			}
			else if (Left is Character && LeftClsnType == ClsnType.Type1Attack && Right is Character && RightClsnType == ClsnType.Type2Normal)
			{
				m_type = CollisionType.CharacterHit;
			}
			else if (Left is Projectile && LeftClsnType == ClsnType.Type1Attack && Right is Character && RightClsnType == ClsnType.Type2Normal)
			{
				m_type = CollisionType.ProjectileHit;
			}
			else if (Left is Projectile && LeftClsnType == ClsnType.Type1Attack && Right is Projectile && RightClsnType == ClsnType.Type1Attack)
			{
				m_type = CollisionType.ProjectileCollision;
			}
		}

		public static Collision Build(Entity lhs, Entity rhs)
		{
			if (lhs == null) throw new ArgumentNullException("lhs");
			if (rhs == null) throw new ArgumentNullException("rhs");

			foreach (Animations.Clsn lhs_clsn in lhs.AnimationManager.CurrentElement)
			{
				Rectangle lhs_rect = lhs_clsn.MakeRect(lhs.CurrentLocation, lhs.CurrentScale, lhs.CurrentFacing);

				foreach (Animations.Clsn rhs_clsn in rhs.AnimationManager.CurrentElement)
				{
					Rectangle rhs_rect = rhs_clsn.MakeRect(rhs.CurrentLocation, rhs.CurrentScale, rhs.CurrentFacing);

					if (lhs_rect.Intersects(rhs_rect) == true) return new Collision(lhs, lhs_clsn.ClsnType, rhs, rhs_clsn.ClsnType);
				}
			}

			return new Collision();
		}

		public static Boolean HasCollision(Entity lhs, ClsnType lhstype, Entity rhs, ClsnType rhstype)
		{
			if (lhs == null) throw new ArgumentNullException("lhs");
			if (rhs == null) throw new ArgumentNullException("rhs");

			foreach (Animations.Clsn lhs_clsn in lhs.AnimationManager.CurrentElement)
			{
				if (lhs_clsn.ClsnType != lhstype) continue;

				Rectangle lhs_rect = lhs_clsn.MakeRect(lhs.CurrentLocation, lhs.CurrentScale, lhs.CurrentFacing);

				foreach (Animations.Clsn rhs_clsn in rhs.AnimationManager.CurrentElement)
				{
					if (rhs_clsn.ClsnType != rhstype) continue;

					Rectangle rhs_rect = rhs_clsn.MakeRect(rhs.CurrentLocation, rhs.CurrentScale, rhs.CurrentFacing);

					if (lhs_rect.Intersects(rhs_rect) == true) return true;
				}
			}

			return false;
		}

		public static Boolean Equals(Collision lhs, Collision rhs)
		{
			if (lhs.Left != rhs.Left) return false;
			if (lhs.LeftClsnType != rhs.LeftClsnType) return false;
			if (lhs.Right != rhs.Right) return false;
			if (lhs.RightClsnType != rhs.RightClsnType) return false;

			return true;
		}

		public Entity Left
		{
			get { return m_lhs; }
		}

		public ClsnType LeftClsnType
		{
			get { return m_lhsclsntype; }
		}

		public Entity Right
		{
			get { return m_rhs; }
		}

		public ClsnType RightClsnType
		{
			get { return m_rhsclsntype; }
		}

		public CollisionType Type
		{
			get { return m_type; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Entity m_lhs;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ClsnType m_lhsclsntype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Entity m_rhs;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ClsnType m_rhsclsntype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CollisionType m_type;

		#endregion
	}
}