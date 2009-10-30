using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	class DefensiveInfo
	{
		public DefensiveInfo(Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			m_character = character;
			m_hitdef = new HitDefinition();
			m_blocked = false;
			m_killed = false;
			m_hitstatetype = StateType.None;
			m_hitshaketime = 0;
			m_defensemultiplier = 1;
			m_attacker = null;
			m_hittime = 0;
			m_hitby1 = new HitBy();
			m_hitby2 = new HitBy();
			m_isfalling = false;

			m_hitoverrides = new List<HitOverride>();
			for (Int32 i = 0; i != 8; ++i) m_hitoverrides.Add(new HitOverride());

			m_hitcount = 0;

		}

		public void Reset()
		{
			m_hitdef.Reset();
			m_blocked = false;
			m_killed = false;
			m_hitstatetype = StateType.None;
			m_hitshaketime = 0;
			m_defensemultiplier = 1;
			m_attacker = null;
			m_hittime = 0;
			m_hitby1.Reset();
			m_hitby2.Reset();
			m_isfalling = false;

			for (Int32 i = 0; i != 8; ++i) m_hitoverrides[i].Reset();

			m_hitcount = 0;
		}

		public void Update()
		{
			HitBy1.Update();
			HitBy2.Update();

			if (HitShakeTime > 0) --HitShakeTime;
			else if (HitTime > -1) --HitTime;

			if (HitShakeTime < 0) HitShakeTime = 0;
			if (HitTime < 0) HitTime = 0;

			if (m_character.StateManager.CurrentState.Number == StateMachine.StateNumber.HitGetUp && m_character.StateManager.StateTime == 0) HitDef.Fall = false;

			foreach (HitOverride hitoverride in m_hitoverrides) hitoverride.Update();
		}

		public void OnHit(HitDefinition hitdef, Character attacker, Boolean blocked)
		{
			if (hitdef == null) throw new ArgumentNullException("hitdef");
			if (attacker == null) throw new ArgumentNullException("attacker");

			Boolean alreadyfalling = IsFalling;

			HitDef.Set(hitdef);
			Attacker = attacker;
			Blocked = blocked;

			if (alreadyfalling == true)
			{
				HitDef.Fall = true;
			}
			else
			{
				m_character.JugglePoints = m_character.BasePlayer.Constants.AirJuggle;
			}

			HitCount = (m_character.MoveType == MoveType.BeingHit) ? HitCount + 1 : 1;
			HitStateType = m_character.StateType;

			m_character.DrawOrder = HitDef.P2SpritePriority;
			m_character.PlayerControl = PlayerControl.NoControl;
			m_character.MoveType = MoveType.BeingHit;

			if (blocked == true)
			{
				HitShakeTime = HitDef.GuardShakeTime;
				m_character.BasePlayer.Power += HitDef.P2GuardPowerAdjustment;
			}
			else
			{
				HitShakeTime = HitDef.ShakeTime;
				m_character.BasePlayer.Power += HitDef.P2HitPowerAdjustment;

				m_character.PaletteFx.Set(HitDef.PalFxTime, HitDef.PalFxAdd, HitDef.PalFxMul, HitDef.PalFxSinAdd, HitDef.PalFxInvert, HitDef.PalFxBaseColor);

				if (IsFalling == true)
				{
					Int32 neededjugglepoints = EvaluationHelper.AsInt32(Attacker, Attacker.StateManager.CurrentState.JugglePoints, 0);
					m_character.JugglePoints -= neededjugglepoints;
				}
			}
		}

		public HitOverride GetOverride(HitDefinition hitdef)
		{
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			foreach (HitOverride hitoverride in HitOverrides)
			{
				if (hitoverride.IsActive == false) continue;

				if (hitoverride.Attribute.HasHeight(hitdef.HitAttribute.AttackHeight) == false) continue;
				foreach (HitType hittype in hitdef.HitAttribute.AttackData) if (hitoverride.Attribute.HasData(hittype) == false) continue;

				return hitoverride;
			}

			return null;
		}

		public Vector2 GetHitVelocity()
		{
			Vector2 velocity;

			if (Blocked == true)
			{
				velocity = (HitStateType == xnaMugen.StateType.Airborne) ? HitDef.AirGuardVelocity : HitDef.GroundGuardVelocity;
			}
			else
			{
				velocity = (HitStateType == xnaMugen.StateType.Airborne) ? HitDef.AirVelocity : HitDef.GroundVelocity;

				if (Killed == true)
				{
					velocity.X *= .66f;
					velocity.Y = -6;
				}
			}

			return velocity;
		}

		public HitDefinition HitDef
		{
			get { return m_hitdef; }
		}

		public Boolean Blocked
		{
			get { return m_blocked; }

			set { m_blocked = value; }
		}

		public Boolean Killed
		{
			get { return m_killed; }

			set { m_killed = value; }
		}

		public StateType HitStateType
		{
			get { return m_hitstatetype; }

			set { m_hitstatetype = value; }
		}

		public Int32 HitShakeTime
		{
			get { return m_hitshaketime; }

			set { m_hitshaketime = value; }
		}

		public Single DefenseMultiplier
		{
			get { return m_defensemultiplier; }

			set { m_defensemultiplier = value; }
		}

		public Character Attacker
		{
			get { return m_attacker; }

			set { m_attacker = value; }
		}

		public Int32 HitTime
		{
			get { return m_hittime; }

			set { m_hittime = value; }
		}

		public HitBy HitBy1
		{
			get { return m_hitby1; }
		}

		public HitBy HitBy2
		{
			get { return m_hitby2; }
		}

		public Boolean IsFalling
		{
			get { return (m_character.MoveType == MoveType.BeingHit) ? HitDef.Fall : false; }
		}

		public ListIterator<HitOverride> HitOverrides
		{
			get { return new ListIterator<HitOverride>(m_hitoverrides); }
		}

		public Int32 HitCount
		{
			get { return m_hitcount; }

			set { m_hitcount = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Character m_character;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HitDefinition m_hitdef;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_blocked;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_killed;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		StateType m_hitstatetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_hitshaketime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_defensemultiplier;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Character m_attacker;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_hittime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HitBy m_hitby1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HitBy m_hitby2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_isfalling;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<HitOverride> m_hitoverrides;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_hitcount;

		#endregion
	}
}