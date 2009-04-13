using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Combat
{
	enum ContactType { None, Hit, Block, MissBlock }

	struct Contact
	{
		public Contact(Character attacker, Character target, HitDefinition hitdef, ContactType type)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");
			if (hitdef == null) throw new ArgumentNullException("hitdef");
			if (type == ContactType.None) throw new ArgumentOutOfRangeException("type");

			m_attacker = attacker;
			m_target = target;
			m_hitdef = hitdef;
			m_type = type;
		}

		public Character Attacker
		{
			get { return m_attacker; }
		}

		public Character Target
		{
			get { return m_target; }
		}

		public HitDefinition HitDef
		{
			get { return m_hitdef; }
		}

		public ContactType Type
		{
			get { return m_type; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Character m_attacker;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Character m_target;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HitDefinition m_hitdef;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ContactType m_type;

		#endregion
	}

	class CombatChecker : EngineObject
	{
		public CombatChecker(FightEngine engine)
			: base(engine)
		{
			m_attacks = new List<Contact>();
			m_killist = new List<Contact>();
			m_attacksorter = ContactSort;
		}

		public void Run()
		{
			RunCharacterAttacks();
			RunProjectileAttacks();
		}

		void RunProjectileAttacks()
		{
			foreach (Entity entity in Engine.Entities)
			{
				Projectile projectile = entity as Projectile;
				if (projectile == null) continue;

				if (projectile.CanAttack() == false) continue;

				foreach (Entity subentity in Engine.Entities)
				{
					if (projectile == subentity) continue;

					if (projectile.CanAttack() == false) break;

					Character character = subentity as Character;
					if (character != null && character != projectile.BasePlayer)
					{
						ProjectileAttack(projectile, character);
					}

					Projectile otherprojectile = subentity as Projectile;
					if (otherprojectile != null)
					{
						if (projectile.Team == otherprojectile.Team) continue;
						if (Collision.HasCollision(projectile, ClsnType.Type1Attack, otherprojectile, ClsnType.Type1Attack) == false) continue;

						ProjectileContact(projectile, otherprojectile);
					}
				}
			}
		}

		void ProjectileAttack(Projectile projectile, Character target)
		{
			if (projectile == null) throw new ArgumentNullException("projectile");
			if (target == null) throw new ArgumentNullException("target");

			if (CanBlock(projectile, target, projectile.Data.HitDef, true) == true)
			{
				OnAttack(projectile, target, projectile.Data.HitDef, true);
			}
			else if (CanHit(projectile, target, projectile.Data.HitDef) == true)
			{
				OnAttack(projectile, target, projectile.Data.HitDef, false);
			}
			else if (CanBlock(projectile, target, projectile.Data.HitDef, false) == true)
			{
				OutOfRangeBlock(target);
			}
		}

		void ProjectileContact(Projectile lhs, Projectile rhs)
		{
			if (lhs == null) throw new ArgumentNullException("lhs");
			if (rhs == null) throw new ArgumentNullException("rhs");

			if (lhs.Priority == rhs.Priority)
			{
				lhs.StartCanceling();
				rhs.StartCanceling();
			}
			else if (lhs.Priority > rhs.Priority)
			{
				--lhs.Priority;
				rhs.StartCanceling();
			}
			else if (lhs.Priority < rhs.Priority)
			{
				lhs.StartCanceling();
				--rhs.Priority;
			}
		}

		void RunCharacterAttacks()
		{
			BuildContacts();

			foreach (Contact attack in m_attacks)
			{
				if (attack.Type == ContactType.Hit)
				{
					if (m_killist.Contains(attack) == true) continue;

					Contact? reverse = FindReverseHit(attack);
					if (reverse != null) PriorityCheck(attack, reverse.Value);

					if (m_killist.Contains(attack) == true) continue;
				}

				RunAttack(attack);
			}

			foreach (Entity entity in Engine.Entities)
			{
				Character character = entity as Character;
				if (character != null)
				{
					Int32 hitcount = CountHits(entity);
					if (hitcount > 0)
					{
						character.OffensiveInfo.HitCount += 1;
						character.OffensiveInfo.UniqueHitCount += hitcount;
						character.OffensiveInfo.ActiveHitDef = false;
					}

					Int32 blockcount = CountBlocks(entity);
					if (blockcount > 0) character.OffensiveInfo.ActiveHitDef = false;
				}
			}
		}

		Int32 CountHits(Entity attacker)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");

			Int32 count = 0;

			foreach (Contact attack in m_attacks)
			{
				if (attack.Attacker == attacker && attack.Type == ContactType.Hit) ++count;
			}

			return count;
		}

		Int32 CountBlocks(Entity attacker)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");

			Int32 count = 0;

			foreach (Contact attack in m_attacks)
			{
				if (attack.Attacker == attacker && attack.Type == ContactType.Block) ++count;
			}

			return count;
		}

		void PriorityCheck(Contact lhs, Contact rhs)
		{
			if (lhs.Type != ContactType.Hit) throw new ArgumentException("lhs");
			if (rhs.Type != ContactType.Hit) throw new ArgumentException("rhs");

			HitPriority lhs_hp = lhs.HitDef.HitPriority;
			HitPriority rhs_hp = rhs.HitDef.HitPriority;

			if (lhs_hp.Power > rhs_hp.Power)
			{
				m_killist.Add(rhs);
			}
			else if (lhs_hp.Power < rhs_hp.Power)
			{
				m_killist.Add(lhs);
			}
			else
			{
				if (lhs_hp.Type != PriorityType.Hit && rhs_hp.Type != PriorityType.Hit)
				{
					m_killist.Add(lhs);
					m_killist.Add(rhs);
				}
				else if (lhs_hp.Type == PriorityType.Dodge || rhs_hp.Type == PriorityType.Dodge)
				{
					m_killist.Add(lhs);
					m_killist.Add(rhs);
				}
				else if (lhs_hp.Type == PriorityType.Hit && rhs_hp.Type == PriorityType.Miss)
				{
					m_killist.Add(rhs);
				}
				else if (lhs_hp.Type == PriorityType.Hit && rhs_hp.Type == PriorityType.Miss)
				{
					m_killist.Add(lhs);
				}
			}
		}

		void RunAttack(Contact attack)
		{
			if (attack.Type == ContactType.Hit)
			{
				OnAttack(attack.Attacker, attack.Target, attack.HitDef, false);
			}
			else if (attack.Type == ContactType.Block)
			{
				OnAttack(attack.Attacker, attack.Target, attack.HitDef, true);
			}
			else if (attack.Type == ContactType.MissBlock)
			{
				OutOfRangeBlock(attack.Target);
			}
		}

		void OnAttack(Projectile projectile, Character target, HitDefinition hitdef, Boolean blocked)
		{
			if (projectile == null) throw new ArgumentNullException("projectile");
			if (target == null) throw new ArgumentNullException("target");
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			Character attacker = projectile.Creator;

			target.DefensiveInfo.OnHit(hitdef, projectile.Creator, blocked);

			hitdef = target.DefensiveInfo.HitDef;

            attacker.OffensiveInfo.AddToTargetList(target);

			projectile.TotalHits += 1;
			projectile.HitCountdown = projectile.Data.TimeBetweenHits;

			if (blocked == true)
			{
				attacker.BasePlayer.Power += hitdef.P1GuardPowerAdjustment;
				attacker.BasePlayer.OffensiveInfo.ProjectileInfo.Set(projectile.Data.ProjectileId, ProjectileDataType.Guarded);

				projectile.HitPauseCountdown = hitdef.GuardPauseTime;

				PlaySound(attacker, target, hitdef.GuardSoundId, hitdef.GuardPlayerSound);
				MakeSpark(projectile, target, hitdef.GuardSparkAnimation, hitdef.SparkStartPosition, hitdef.GuardPlayerSpark);
			}
			else
			{
				attacker.BasePlayer.Power += hitdef.P1HitPowerAdjustment;
				attacker.BasePlayer.OffensiveInfo.ProjectileInfo.Set(projectile.Data.ProjectileId, ProjectileDataType.Hit);

				projectile.HitPauseCountdown = hitdef.PauseTime;

				DoEnvShake(hitdef, attacker.Engine.EnvironmentShake);
				PlaySound(attacker, target, hitdef.HitSoundId, hitdef.PlayerSound);
				MakeSpark(projectile, target, hitdef.SparkAnimation, hitdef.SparkStartPosition, hitdef.PlayerSpark);
			}

			HitOverride hitoverride = target.DefensiveInfo.GetOverride(hitdef);
			if (hitoverride != null)
			{
				if (hitoverride.ForceAir == true) target.DefensiveInfo.IsFalling = true;

				target.StateManager.ForeignManager = null;
				target.StateManager.ChangeState(hitoverride.StateNumber);
			}
			else
			{
				if (blocked == false)
				{
                    OnAttackHit(attacker, target, hitdef);
				}
				else
				{
                    OnAttackBlock(attacker, target, hitdef);
                }
			}
		}

		void OnAttack(Character attacker, Character target, HitDefinition hitdef, Boolean blocked)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			target.DefensiveInfo.OnHit(hitdef, attacker, blocked);

			HitDefinition myhitdef = target.DefensiveInfo.HitDef;

			attacker.OffensiveInfo.OnHit(myhitdef, target, blocked);

			SetFacing(attacker, target, myhitdef);

			if (blocked == false)
			{
				DoEnvShake(myhitdef, attacker.Engine.EnvironmentShake);
				PlaySound(attacker, target, myhitdef.HitSoundId, myhitdef.PlayerSound);
				MakeSpark(attacker, target, myhitdef.SparkAnimation, myhitdef.SparkStartPosition, myhitdef.PlayerSpark);
			}
			else
			{
				PlaySound(attacker, target, myhitdef.GuardSoundId, myhitdef.GuardPlayerSound);
				MakeSpark(attacker, target, myhitdef.GuardSparkAnimation, myhitdef.SparkStartPosition, myhitdef.GuardPlayerSpark);
			}

			HitOverride hitoverride = target.DefensiveInfo.GetOverride(myhitdef);
			if (hitoverride != null)
			{
				if (hitoverride.ForceAir == true) target.DefensiveInfo.IsFalling = true;

				target.StateManager.ForeignManager = null;
				target.StateManager.ChangeState(hitoverride.StateNumber);
			}
			else
			{
				if (blocked == false)
				{
					OnAttackHit(attacker, target, myhitdef);
				}
				else
				{
					OnAttackBlock(attacker, target, myhitdef);
				}
			}
		}

		void OnAttackBlock(Character attacker, Character target, HitDefinition hitdef)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			target.DefensiveInfo.HitTime = hitdef.GuardHitTime;
			ApplyDamage(attacker, target, hitdef.GuardDamage, hitdef.CanGuardKill);

			switch (target.DefensiveInfo.HitStateType)
			{
				case StateType.Standing:
					target.StateManager.ChangeState(StateMachine.StateNumber.StandingGuardHitShaking);
					break;

				case StateType.Airborne:
					target.StateManager.ChangeState(StateMachine.StateNumber.AirGuardHitShaking);
					break;

				case StateType.Crouching:
					target.StateManager.ChangeState(StateMachine.StateNumber.CrouchingGuardHitShaking);
					break;

				default:
					throw new ArgumentOutOfRangeException("target.DefensiveInfo.HitStateType");
			}
		}

		void OnAttackHit(Character attacker, Character target, HitDefinition hitdef)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			ApplyDamage(attacker, target, hitdef.HitDamage, hitdef.CanKill);

			if (target.Life == 0)
			{
				target.DefensiveInfo.Killed = true;
				target.DefensiveInfo.IsFalling = true;
			}

			switch (target.DefensiveInfo.HitStateType)
			{
				case StateType.Standing:
				case StateType.Crouching:
				case StateType.Prone:
					target.DefensiveInfo.HitTime = hitdef.GroundHitTime;
					break;

				case StateType.Airborne:
					target.DefensiveInfo.HitTime = hitdef.AirHitTime;
					break;

				default:
					throw new ArgumentOutOfRangeException("target.DefensiveInfo.HitStateType");
			}

			if (hitdef.P1NewState != null)
			{
				attacker.StateManager.ChangeState(hitdef.P1NewState.Value);
			}

			if (hitdef.P2NewState != null)
			{
				if (hitdef.P2UseP1State == true)
				{
					target.StateManager.ForeignManager = attacker.StateManager;
					target.StateManager.ChangeState(hitdef.P2NewState.Value);
				}
				else
				{
					target.StateManager.ForeignManager = null;
					target.StateManager.ChangeState(hitdef.P2NewState.Value);
				}
			}
			else
			{
				target.StateManager.ForeignManager = null;

				if (hitdef.GroundAttackEffect == AttackEffect.Trip)
				{
					target.StateManager.ChangeState(StateMachine.StateNumber.HitTrip);
				}
				else
				{
					switch (target.DefensiveInfo.HitStateType)
					{
						case StateType.Standing:
							target.StateManager.ChangeState(StateMachine.StateNumber.StandingHitShaking);
							break;

						case StateType.Crouching:
							target.StateManager.ChangeState(StateMachine.StateNumber.CrouchingHitShaking);
							break;

						case StateType.Airborne:
							target.StateManager.ChangeState(StateMachine.StateNumber.AirHitShaking);
							break;

                        case StateType.Prone:
                            target.StateManager.ChangeState(StateMachine.StateNumber.HitProneShaking);
                            break;

						default:
							throw new ArgumentOutOfRangeException("target.DefensiveInfo.HitStateType");
					}
				}
			}
		}

		void SetFacing(Character attacker, Character target, HitDefinition hitdef)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			if (hitdef.P1Facing == -1) attacker.CurrentFacing = Misc.FlipFacing(attacker.CurrentFacing);

			if (hitdef.P1GetP2Facing == -1) attacker.CurrentFacing = Misc.FlipFacing(target.CurrentFacing);

			if (hitdef.P1GetP2Facing == 1) attacker.CurrentFacing = target.CurrentFacing;

			if (hitdef.P2Facing == 1) target.CurrentFacing = Misc.FlipFacing(attacker.CurrentFacing);

			if (hitdef.P2Facing == -1) target.CurrentFacing = attacker.CurrentFacing;
		}

		void DoEnvShake(HitDefinition hitdef, EnvironmentShake envshake)
		{
			if (hitdef == null) throw new ArgumentNullException("hitdef");
			if (envshake == null) throw new ArgumentNullException("envshake");

			if (hitdef.EnvShakeTime == 0) return;

			envshake.Set(hitdef.EnvShakeTime, hitdef.EnvShakeFrequency, hitdef.EnvShakeAmplitude, hitdef.EnvShakePhase);
		}

		void ApplyDamage(Character attacker, Character target, Int32 amount, Boolean cankill)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");

			Single offensive_multiplier = attacker.OffensiveInfo.AttackMultiplier * (attacker.BasePlayer.Constants.AttackPower / 100.0f);
			Single defensive_multiplier = target.DefensiveInfo.DefenseMultiplier * (target.BasePlayer.Constants.DefensivePower / 100.0f);

			amount = (Int32)(amount * offensive_multiplier / defensive_multiplier);

			target.Life -= amount;
			if (target.Life == 0 && cankill == false) target.Life = 1;
		}

		void PlaySound(Character attacker, Character target, SoundId soundid, Boolean playersound)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");

			Audio.SoundManager snd = (playersound == true) ? attacker.SoundManager : attacker.Engine.CommonSounds;
			snd.Play(-1, soundid, false, 0, 1.0f, false);
		}

		void MakeSpark(Projectile projectile, Character target, Int32 animationnumber, Vector2 sparklocation, Boolean playeranimation)
		{
			if (projectile == null) throw new ArgumentNullException("projectile");
			if (target == null) throw new ArgumentNullException("target");

			ExplodData data = new ExplodData();
            data.IsHitSpark = true;
			data.CommonAnimation = playeranimation == false;
			data.PositionType = PositionType.P1;
			data.AnimationNumber = animationnumber;
			data.SpritePriority = projectile.DrawOrder + 1;
			data.RemoveTime = -2;
			data.OwnPalFx = false;
			data.Scale = Vector2.One;
			data.Location = sparklocation;
			data.Creator = projectile.Creator;
			data.Offseter = projectile;

			Explod explod = new Explod(projectile.Engine, data);
			if (explod.IsValid == true) explod.Engine.Entities.Add(explod);
		}

		void MakeSpark(Character attacker, Character target, Int32 animationnumber, Vector2 sparklocation, Boolean playeranimation)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");

			ExplodData data = new ExplodData();
            data.IsHitSpark = true;
            data.CommonAnimation = playeranimation == false;
			data.PositionType = PositionType.P1;
			data.AnimationNumber = animationnumber;
			data.SpritePriority = attacker.DrawOrder + 1;
			data.RemoveTime = -2;
			data.OwnPalFx = false;
			data.Scale = Vector2.One;
			data.Location = GetSparkLocation(attacker, target, sparklocation);
			data.Creator = attacker;
			data.Offseter = target;
			data.Flip = SpriteEffects.FlipHorizontally;

			Explod explod = new Explod(attacker.Engine, data);
			if (explod.IsValid == true) explod.Engine.Entities.Add(explod);
		}

		Vector2 GetSparkLocation(Character attacker, Character target, Vector2 baselocation)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");

			Vector2 offset = new Vector2(0, attacker.CurrentLocation.Y - target.CurrentLocation.Y);

			switch (target.CurrentFacing)
			{
				case Facing.Left:
					offset.X = target.CurrentLocation.X - target.GetFrontLocation();
					break;

				case Facing.Right:
					offset.X = target.GetFrontLocation() - target.CurrentLocation.X;
					break;
			}

			return baselocation + offset;
		}

		void OutOfRangeBlock(Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			Int32 currentstatenumber = character.StateManager.CurrentState.Number;
			if (currentstatenumber < StateMachine.StateNumber.GuardStart || currentstatenumber > StateMachine.StateNumber.GuardEnd)
			{
				character.StateManager.ChangeState(StateMachine.StateNumber.GuardStart);
			}
		}

		void BuildContacts()
		{
			m_attacks.Clear();
			m_killist.Clear();

			foreach (Entity entity in Engine.Entities)
			{
				Character character = entity as Character;
				if (character != null) HitCheck(character);
			}

			m_attacks.Sort(m_attacksorter);
		}

		void HitCheck(Character attacker)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");

			if (attacker.InHitPause == true || attacker.OffensiveInfo.ActiveHitDef == false) return;

			foreach (Entity entity in Engine.Entities)
			{
				Character target = entity as Character;
				if (target == null || target == attacker) continue;

				if (attacker.Assertions.UnGuardable == false && CanBlock(attacker, target, attacker.OffensiveInfo.HitDef, true) == true)
				{
					m_attacks.Add(new Contact(attacker, target, attacker.OffensiveInfo.HitDef, ContactType.Block));
				}
				else if (CanHit(attacker, target, attacker.OffensiveInfo.HitDef) == true)
				{
					m_attacks.Add(new Contact(attacker, target, attacker.OffensiveInfo.HitDef, ContactType.Hit));
				}
				else if (CanBlock(attacker, target, attacker.OffensiveInfo.HitDef, false) == true && target is Player)
				{
					m_attacks.Add(new Contact(attacker, target, attacker.OffensiveInfo.HitDef, ContactType.MissBlock));
				}
			}
		}

		Int32 ContactSort(Contact lhs, Contact rhs)
		{
			if (lhs.Type != rhs.Type) return Comparer<ContactType>.Default.Compare(lhs.Type, rhs.Type);

			if (lhs.Type == ContactType.Hit)
			{
				HitPriority hp_lhs = lhs.HitDef.HitPriority;
				HitPriority hp_rhs = rhs.HitDef.HitPriority;

				if (hp_lhs.Power == hp_rhs.Power) return 0;

				return (hp_lhs.Power < hp_rhs.Power) ? -1 : 1;
			}

			return 0;
		}

		Contact? FindReverseHit(Contact attack)
		{
			if (attack.Type != ContactType.Hit) throw new ArgumentOutOfRangeException("attack");

			foreach (Contact iter in m_attacks)
			{
				if (m_killist.Contains(iter) == true) continue;

				if (iter.Target == attack.Attacker && iter.Attacker == attack.Target)
				{
					return iter;
				}
			}

			return null;
		}

		Boolean CanHit(Entity attacker, Character target, HitDefinition hitdef)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			if (Collision.HasCollision(attacker, ClsnType.Type1Attack, target, ClsnType.Type2Normal) == false) return false;

			if (hitdef.Targeting == AffectTeam.None) return false;
			if (((hitdef.Targeting & AffectTeam.Enemy) != AffectTeam.Enemy) && (attacker.Team != target.Team)) return false;
			if (((hitdef.Targeting & AffectTeam.Friendly) != AffectTeam.Friendly) && (attacker.Team == target.Team)) return false;

			if (target.StateType == StateType.Standing && hitdef.HitFlag.HitHigh == false) return false;
			if (target.StateType == StateType.Crouching && hitdef.HitFlag.HitLow == false) return false;
			if (target.StateType == StateType.Airborne && hitdef.HitFlag.HitAir == false) return false;
			if (target.StateType == StateType.Prone && hitdef.HitFlag.HitDown == false) return false;
			if (target.MoveType == MoveType.BeingHit && hitdef.HitFlag.ComboFlag == HitFlagCombo.No) return false;
			if (target.MoveType != MoveType.BeingHit && hitdef.HitFlag.ComboFlag == HitFlagCombo.Yes) return false;

			if (target.DefensiveInfo.HitBy1.CanHit(hitdef.HitAttribute) == false) return false;
			if (target.DefensiveInfo.HitBy2.CanHit(hitdef.HitAttribute) == false) return false;

			if (target.DefensiveInfo.IsFalling == true)
			{
				if (attacker is Player)
				{
					Player player = attacker as Player;

#warning Juggling system is weird
					Int32 neededjugglepoints = EvaluationHelper.AsInt32(player, player.StateManager.CurrentState.JugglePoints, 0);
					//if (neededjugglepoints > target.JugglePoints) return false;
				}
			}

			return true;
		}

		Boolean CanBlock(Entity attacker, Character target, HitDefinition hitdef, Boolean rangecheck)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			if (rangecheck == true && Collision.HasCollision(attacker, ClsnType.Type1Attack, target, ClsnType.Type2Normal) == false) return false;

			if (hitdef.Targeting == AffectTeam.None) return false;
			if (((hitdef.Targeting & AffectTeam.Enemy) != AffectTeam.Enemy) && (attacker.Team != target.Team)) return false;
			if (((hitdef.Targeting & AffectTeam.Friendly) != AffectTeam.Friendly) && (attacker.Team == target.Team)) return false;

			if (target.CommandManager.IsActive("holdback") == false) return false;
			if (InGuardDistance(attacker, target, hitdef) == false) return false;

			if (target.StateType == StateType.Airborne && (hitdef.GuardFlag.HitAir == false || target.Assertions.NoAirGuard == true)) return false;
			if (target.StateType == StateType.Standing && (hitdef.GuardFlag.HitHigh == false || target.Assertions.NoStandingGuard == true)) return false;
			if (target.StateType == StateType.Crouching && (hitdef.GuardFlag.HitLow == false || target.Assertions.NoCrouchingGuard == true)) return false;
			if (target.StateType == StateType.Prone) return false;

			if (target.Life <= hitdef.GuardDamage && hitdef.CanGuardKill == true) return false;

			return true;
		}

		Boolean InGuardDistance(Entity attacker, Character target, HitDefinition hitdef)
		{
			if (attacker == null) throw new ArgumentNullException("attacker");
			if (target == null) throw new ArgumentNullException("target");
			if (hitdef == null) throw new ArgumentNullException("hitdef");

			Single distance = Math.Abs(attacker.CurrentLocation.X - target.CurrentLocation.X);

			return distance <= hitdef.GuardDistance;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Contact> m_attacks;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Contact> m_killist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Comparison<Contact> m_attacksorter;

		#endregion
	}
}