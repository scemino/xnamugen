using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Combat
{
	internal class OffensiveInfo
	{
		public OffensiveInfo(Character character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			m_character = character;
			m_hitdef = new HitDefinition();
			m_hitpausetime = 0;
			m_isactive = false;
			m_movecontact = 0;
			m_moveguarded = 0;
			m_movehit = 0;
			m_movereversed = 0;
			m_attackmultiplier = 1;
			m_hitcount = 0;
			m_uniquehitcount = 0;
			m_projectileinfo = new ProjectileInfo();
			m_targetlist = new List<Character>();
		}

		public void Reset()
		{
			m_hitdef.Reset();
			m_hitpausetime = 0;
			m_isactive = false;
			m_movecontact = 0;
			m_moveguarded = 0;
			m_movehit = 0;
			m_movereversed = 0;
			m_attackmultiplier = 1;
			m_hitcount = 0;
			m_uniquehitcount = 0;
			m_projectileinfo.Reset();
			m_targetlist.Clear();
		}

		public void Update()
		{
			ProjectileInfo.Update();

			if (MoveContact > 0) ++MoveContact;
			if (MoveHit > 0) ++MoveHit;
			if (MoveGuarded > 0) ++MoveGuarded;
			if (MoveReversed > 0) ++MoveReversed;
		}

		public void OnHit(HitDefinition hitdef, Character target, bool blocked)
		{
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));
			if (target == null) throw new ArgumentNullException(nameof(target));

			m_character.DrawOrder = hitdef.P1SpritePriority;

			AddToTargetList(target);

			if (blocked)
			{
				m_character.BasePlayer.Power += hitdef.P1GuardPowerAdjustment;
				HitPauseTime = hitdef.GuardPauseTime;
				MoveContact = 1;
				MoveGuarded = 1;
				MoveHit = 0;
				MoveReversed = 0;
			}
			else
			{
				m_character.BasePlayer.Power += hitdef.P1HitPowerAdjustment;
				HitPauseTime = hitdef.PauseTime;
				MoveContact = 1;
				MoveGuarded = 0;
				MoveHit = 1;
				MoveReversed = 0;
			}
		}

		public void AddToTargetList(Character target)
		{
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (target == m_character) throw new ArgumentOutOfRangeException(nameof(target));

			if (TargetList.Contains(target) == false) TargetList.Add(target);
		}

		public HitDefinition HitDef => m_hitdef;

		public bool ActiveHitDef
		{
			get => m_isactive;

			set { m_isactive = value; }
		}

		public int HitPauseTime
		{
			get => m_hitpausetime;

			set { m_hitpausetime = value; }
		}

		public int MoveContact
		{
			get => m_movecontact;

			set { m_movecontact = value; }
		}

		public int MoveHit
		{
			get => m_movehit;

			set { m_movehit = value; }
		}

		public int MoveGuarded
		{
			get => m_moveguarded;

			set { m_moveguarded = value; }
		}

		public int MoveReversed
		{
			get => m_movereversed;

			set { m_movereversed = value; }
		}

		public float AttackMultiplier
		{
			get => m_attackmultiplier;

			set { m_attackmultiplier = value; }
		}

		public int HitCount
		{
			get => m_hitcount;

			set { m_hitcount = value; }
		}

		public int UniqueHitCount
		{
			get => m_uniquehitcount;

			set { m_uniquehitcount = value; }
		}

		public ProjectileInfo ProjectileInfo => m_projectileinfo;

		public List<Character> TargetList => m_targetlist;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Character m_character;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly HitDefinition m_hitdef;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_hitpausetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_movehit;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_moveguarded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_movecontact;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_movereversed;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float m_attackmultiplier;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_hitcount;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_uniquehitcount;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ProjectileInfo m_projectileinfo;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<Character> m_targetlist;

		#endregion
	}
}