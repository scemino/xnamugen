using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal struct Collision
	{
		public Collision(Entity lhs, ClsnType lhstype, Entity rhs, ClsnType rhstype)
		{
			if (lhs == null) throw new ArgumentNullException(nameof(lhs));
			if (rhs == null) throw new ArgumentNullException(nameof(rhs));

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
			if (lhs == null) throw new ArgumentNullException(nameof(lhs));
			if (rhs == null) throw new ArgumentNullException(nameof(rhs));

			foreach (var lhs_clsn in lhs.AnimationManager.CurrentElement)
			{
				var lhs_rect = lhs_clsn.MakeRect(lhs.CurrentLocation, lhs.CurrentScale, lhs.CurrentFacing);

				foreach (var rhs_clsn in rhs.AnimationManager.CurrentElement)
				{
					var rhs_rect = rhs_clsn.MakeRect(rhs.CurrentLocation, rhs.CurrentScale, rhs.CurrentFacing);

					if (lhs_rect.Intersects(rhs_rect)) return new Collision(lhs, lhs_clsn.ClsnType, rhs, rhs_clsn.ClsnType);
				}
			}

			return new Collision();
		}

		public static bool HasCollision(Entity lhs, ClsnType lhstype, Entity rhs, ClsnType rhstype)
		{
			if (lhs == null) throw new ArgumentNullException(nameof(lhs));
			if (rhs == null) throw new ArgumentNullException(nameof(rhs));

			foreach (var lhs_clsn in lhs.AnimationManager.CurrentElement)
			{
				if (lhs_clsn.ClsnType != lhstype) continue;

				var lhs_rect = lhs_clsn.MakeRect(lhs.CurrentLocation, lhs.CurrentScale, lhs.CurrentFacing);

				foreach (var rhs_clsn in rhs.AnimationManager.CurrentElement)
				{
					if (rhs_clsn.ClsnType != rhstype) continue;

					var rhs_rect = rhs_clsn.MakeRect(rhs.CurrentLocation, rhs.CurrentScale, rhs.CurrentFacing);

					if (lhs_rect.Intersects(rhs_rect)) return true;
				}
			}

			return false;
		}

		public static bool Equals(Collision lhs, Collision rhs)
		{
			if (lhs.Left != rhs.Left) return false;
			if (lhs.LeftClsnType != rhs.LeftClsnType) return false;
			if (lhs.Right != rhs.Right) return false;
			if (lhs.RightClsnType != rhs.RightClsnType) return false;

			return true;
		}

		public Entity Left => m_lhs;

		public ClsnType LeftClsnType => m_lhsclsntype;

		public Entity Right => m_rhs;

		public ClsnType RightClsnType => m_rhsclsntype;

		public CollisionType Type => m_type;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Entity m_lhs;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ClsnType m_lhsclsntype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Entity m_rhs;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ClsnType m_rhsclsntype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CollisionType m_type;

		#endregion
	}
}