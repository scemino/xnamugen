using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Combat
{
	class OffensiveInfo
	{
		public OffensiveInfo(Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

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

		public void OnHit(HitDefinition hitdef, Character target, Boolean blocked)
		{
			if (hitdef == null) throw new ArgumentNullException("hitdef");
			if (target == null) throw new ArgumentNullException("target");

			m_character.DrawOrder = hitdef.P1SpritePriority;

			AddToTargetList(target);

			if (blocked == true)
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
			if (target == null) throw new ArgumentNullException("target");
			if (target == m_character) throw new ArgumentOutOfRangeException("target");

			if (TargetList.Contains(target) == false) TargetList.Add(target);
		}

		public HitDefinition HitDef
		{
			get { return m_hitdef; }
		}

		public Boolean ActiveHitDef
		{
			get { return m_isactive; }

			set { m_isactive = value; }
		}

		public Int32 HitPauseTime
		{
			get { return m_hitpausetime; }

			set { m_hitpausetime = value; }
		}

		public Int32 MoveContact
		{
			get { return m_movecontact; }

			set { m_movecontact = value; }
		}

		public Int32 MoveHit
		{
			get { return m_movehit; }

			 set { m_movehit = value; }
		}

		public Int32 MoveGuarded
		{
			get { return m_moveguarded; }

			set { m_moveguarded = value; }
		}

		public Int32 MoveReversed
		{
			get { return m_movereversed; }

			set { m_movereversed = value; }
		}

		public Single AttackMultiplier
		{
			get { return m_attackmultiplier; }

			set { m_attackmultiplier = value; }
		}

		public Int32 HitCount
		{
			get { return m_hitcount; }

			set { m_hitcount = value; }
		}

		public Int32 UniqueHitCount
		{
			get { return m_uniquehitcount; }

			set { m_uniquehitcount = value; }
		}

		public ProjectileInfo ProjectileInfo
		{
			get { return m_projectileinfo; }
		}

		public List<Character> TargetList
		{
			get { return m_targetlist; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Character m_character;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HitDefinition m_hitdef;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_isactive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_hitpausetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_movehit;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_moveguarded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_movecontact;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_movereversed;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_attackmultiplier;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_hitcount;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_uniquehitcount;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ProjectileInfo m_projectileinfo;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		List<Character> m_targetlist;

		#endregion
	}
}