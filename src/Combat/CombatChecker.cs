using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	internal enum ContactType { None, Hit, Block, MissBlock }

	internal struct Contact
	{
		public Contact(Character attacker, Character target, HitDefinition hitdef, ContactType type)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));
			if (type == ContactType.None) throw new ArgumentOutOfRangeException(nameof(type));

			m_attacker = attacker;
			m_target = target;
			m_hitdef = hitdef;
			m_type = type;
		}

		public Character Attacker => m_attacker;

		public Character Target => m_target;

		public HitDefinition HitDef => m_hitdef;

		public ContactType Type => m_type;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Character m_attacker;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Character m_target;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly HitDefinition m_hitdef;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ContactType m_type;

		#endregion
	}

	internal class CombatChecker : EngineObject
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

		private void RunProjectileAttacks()
		{
			foreach (var entity in Engine.Entities)
			{
				var projectile = entity as Projectile;
				if (projectile == null) continue;

				if (projectile.CanAttack() == false) continue;

				foreach (var subentity in Engine.Entities)
				{
					if (projectile == subentity) continue;

					if (projectile.CanAttack() == false) break;

					var character = subentity as Character;
					if (character != null)
					{
						ProjectileAttack(projectile, character);
					}

					var otherprojectile = subentity as Projectile;
					if (otherprojectile != null)
					{
						if (projectile.Team == otherprojectile.Team) continue;
						if (Collision.HasCollision(projectile, ClsnType.Type1Attack, otherprojectile, ClsnType.Type1Attack) == false) continue;

						ProjectileContact(projectile, otherprojectile);
					}
				}
			}
		}

		private void ProjectileAttack(Projectile projectile, Character target)
		{
			if (projectile == null) throw new ArgumentNullException(nameof(projectile));
			if (target == null) throw new ArgumentNullException(nameof(target));

			if (CanBlock(projectile, target, projectile.Data.HitDef, true))
			{
				OnAttack(projectile, target, projectile.Data.HitDef, true);
			}
			else if (CanHit(projectile, target, projectile.Data.HitDef))
			{
				OnAttack(projectile, target, projectile.Data.HitDef, false);
			}
			else if (CanBlock(projectile, target, projectile.Data.HitDef, false))
			{
				OutOfRangeBlock(target);
			}
		}

		private void ProjectileContact(Projectile lhs, Projectile rhs)
		{
			if (lhs == null) throw new ArgumentNullException(nameof(lhs));
			if (rhs == null) throw new ArgumentNullException(nameof(rhs));

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

		private void RunCharacterAttacks()
		{
			BuildContacts();

			foreach (var attack in m_attacks)
			{
				if (attack.Type == ContactType.Hit)
				{
					if (m_killist.Contains(attack)) continue;

					var reverse = FindReverseHit(attack);
					if (reverse != null) PriorityCheck(attack, reverse.Value);

					if (m_killist.Contains(attack)) continue;
				}

				RunAttack(attack);
			}

			foreach (var entity in Engine.Entities)
			{
				var character = entity as Character;
				if (character != null)
				{
					var hitcount = CountHits(entity);
					if (hitcount > 0)
					{
						character.OffensiveInfo.HitCount += 1;
						character.OffensiveInfo.UniqueHitCount += hitcount;
						character.OffensiveInfo.ActiveHitDef = false;
					}

					var blockcount = CountBlocks(entity);
					if (blockcount > 0) character.OffensiveInfo.ActiveHitDef = false;
				}
			}
		}

		private int CountHits(Entity attacker)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));

			var count = 0;

			foreach (var attack in m_attacks)
			{
				if (attack.Attacker == attacker && attack.Type == ContactType.Hit) ++count;
			}

			return count;
		}

		private int CountBlocks(Entity attacker)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));

			var count = 0;

			foreach (var attack in m_attacks)
			{
				if (attack.Attacker == attacker && attack.Type == ContactType.Block) ++count;
			}

			return count;
		}

		private void PriorityCheck(Contact lhs, Contact rhs)
		{
			if (lhs.Type != ContactType.Hit) throw new ArgumentException("lhs");
			if (rhs.Type != ContactType.Hit) throw new ArgumentException("rhs");

			var lhs_hp = lhs.HitDef.HitPriority;
			var rhs_hp = rhs.HitDef.HitPriority;

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

		private void RunAttack(Contact attack)
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

		private void OnAttack(Projectile projectile, Character target, HitDefinition hitdef, bool blocked)
		{
			if (projectile == null) throw new ArgumentNullException(nameof(projectile));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));

			var attacker = projectile.Creator;

			target.DefensiveInfo.OnHit(hitdef, projectile.Creator, blocked);

			hitdef = target.DefensiveInfo.HitDef;

			projectile.TotalHits += 1;
			projectile.HitCountdown = projectile.Data.TimeBetweenHits;

			if (blocked)
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

			var hitoverride = target.DefensiveInfo.GetOverride(hitdef);
			if (hitoverride != null)
			{
				if (hitoverride.ForceAir) hitdef.Fall = true;

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

		private void OnAttack(Character attacker, Character target, HitDefinition hitdef, bool blocked)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));

			target.DefensiveInfo.OnHit(hitdef, attacker, blocked);

			var myhitdef = target.DefensiveInfo.HitDef;

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

			var hitoverride = target.DefensiveInfo.GetOverride(myhitdef);
			if (hitoverride != null)
			{
				if (hitoverride.ForceAir) myhitdef.Fall = true;

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

		private void OnAttackBlock(Character attacker, Character target, HitDefinition hitdef)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));

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

		private void OnAttackHit(Character attacker, Character target, HitDefinition hitdef)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));

			ApplyDamage(attacker, target, hitdef.HitDamage, hitdef.CanKill);

			if (target.Life == 0)
			{
				target.DefensiveInfo.Killed = true;
				hitdef.Fall = true;
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
				if (hitdef.P2UseP1State)
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

		private void SetFacing(Character attacker, Character target, HitDefinition hitdef)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));

			if (hitdef.P1Facing == -1) attacker.CurrentFacing = Misc.FlipFacing(attacker.CurrentFacing);

			if (hitdef.P1GetP2Facing == -1) attacker.CurrentFacing = Misc.FlipFacing(target.CurrentFacing);

			if (hitdef.P1GetP2Facing == 1) attacker.CurrentFacing = target.CurrentFacing;

			if (hitdef.P2Facing == 1) target.CurrentFacing = Misc.FlipFacing(attacker.CurrentFacing);

			if (hitdef.P2Facing == -1) target.CurrentFacing = attacker.CurrentFacing;
		}

		private void DoEnvShake(HitDefinition hitdef, EnvironmentShake envshake)
		{
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));
			if (envshake == null) throw new ArgumentNullException(nameof(envshake));

			if (hitdef.EnvShakeTime == 0) return;

			envshake.Set(hitdef.EnvShakeTime, hitdef.EnvShakeFrequency, hitdef.EnvShakeAmplitude, hitdef.EnvShakePhase);
		}

		private void ApplyDamage(Character attacker, Character target, int amount, bool cankill)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));

			var offensive_multiplier = attacker.OffensiveInfo.AttackMultiplier * (attacker.BasePlayer.Constants.AttackPower / 100.0f);
			var defensive_multiplier = target.DefensiveInfo.DefenseMultiplier * (target.BasePlayer.Constants.DefensivePower / 100.0f);

			amount = (int)(amount * offensive_multiplier / defensive_multiplier);

			target.Life -= amount;
			if (target.Life == 0 && cankill == false) target.Life = 1;
		}

		private void PlaySound(Character attacker, Character target, SoundId soundid, bool playersound)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));

			//Audio.SoundManager snd = (playersound == true) ? attacker.SoundManager : attacker.Engine.CommonSounds;
			//snd.Play(-1, soundid, false, 0, 1.0f, false);
		}

		private void MakeSpark(Projectile projectile, Character target, int animationnumber, Vector2 sparklocation, bool playeranimation)
		{
			if (projectile == null) throw new ArgumentNullException(nameof(projectile));
			if (target == null) throw new ArgumentNullException(nameof(target));

			var data = new ExplodData();
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

			var explod = new Explod(projectile.Engine, data);
			if (explod.IsValid) explod.Engine.Entities.Add(explod);
		}

		private void MakeSpark(Character attacker, Character target, int animationnumber, Vector2 sparklocation, bool playeranimation)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));

			var data = new ExplodData();
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

			var explod = new Explod(attacker.Engine, data);
			if (explod.IsValid) explod.Engine.Entities.Add(explod);
		}

		private Vector2 GetSparkLocation(Character attacker, Character target, Vector2 baselocation)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));

			var offset = new Vector2(0, attacker.CurrentLocation.Y - target.CurrentLocation.Y);

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

		private void OutOfRangeBlock(Character character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			var currentstatenumber = character.StateManager.CurrentState.Number;
			if (currentstatenumber < StateMachine.StateNumber.GuardStart || currentstatenumber > StateMachine.StateNumber.GuardEnd)
			{
				character.StateManager.ChangeState(StateMachine.StateNumber.GuardStart);
			}
		}

		private void BuildContacts()
		{
			m_attacks.Clear();
			m_killist.Clear();

			foreach (var entity in Engine.Entities)
			{
				var character = entity as Character;
				if (character != null) HitCheck(character);
			}

			m_attacks.Sort(m_attacksorter);
		}

		private void HitCheck(Character attacker)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));

			if (attacker.InHitPause || attacker.OffensiveInfo.ActiveHitDef == false) return;

			foreach (var entity in Engine.Entities)
			{
				var target = entity as Character;
				if (target == null) continue;

				if (CanBlock(attacker, target, attacker.OffensiveInfo.HitDef, true))
				{
					m_attacks.Add(new Contact(attacker, target, attacker.OffensiveInfo.HitDef, ContactType.Block));
				}
				else if (CanHit(attacker, target, attacker.OffensiveInfo.HitDef))
				{
					m_attacks.Add(new Contact(attacker, target, attacker.OffensiveInfo.HitDef, ContactType.Hit));
				}
				else if (CanBlock(attacker, target, attacker.OffensiveInfo.HitDef, false))
				{
					m_attacks.Add(new Contact(attacker, target, attacker.OffensiveInfo.HitDef, ContactType.MissBlock));
				}
			}
		}

		private int ContactSort(Contact lhs, Contact rhs)
		{
			if (lhs.Type != rhs.Type) return Comparer<ContactType>.Default.Compare(lhs.Type, rhs.Type);

			if (lhs.Type == ContactType.Hit)
			{
				var hp_lhs = lhs.HitDef.HitPriority;
				var hp_rhs = rhs.HitDef.HitPriority;

				if (hp_lhs.Power == hp_rhs.Power) return 0;

				return hp_lhs.Power < hp_rhs.Power ? -1 : 1;
			}

			return 0;
		}

		private Contact? FindReverseHit(Contact attack)
		{
			if (attack.Type != ContactType.Hit) throw new ArgumentOutOfRangeException(nameof(attack));

			foreach (var iter in m_attacks)
			{
				if (m_killist.Contains(iter)) continue;

				if (iter.Target == attack.Attacker && iter.Attacker == attack.Target)
				{
					return iter;
				}
			}

			return null;
		}

		private bool CanHit(Entity attacker, Character target, HitDefinition hitdef)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));

			if (Collision.HasCollision(attacker, ClsnType.Type1Attack, target, ClsnType.Type2Normal) == false) return false;

			if (hitdef.Targeting == AffectTeam.None) return false;
			if ((hitdef.Targeting & AffectTeam.Enemy) != AffectTeam.Enemy && attacker.Team != target.Team) return false;
			if ((hitdef.Targeting & AffectTeam.Friendly) != AffectTeam.Friendly && attacker.Team == target.Team) return false;

			if (target.StateType == StateType.Standing && hitdef.HitFlag.HitHigh == false) return false;
			if (target.StateType == StateType.Crouching && hitdef.HitFlag.HitLow == false) return false;
			if (target.StateType == StateType.Airborne && hitdef.HitFlag.HitAir == false) return false;
			if (target.StateType == StateType.Prone && hitdef.HitFlag.HitDown == false) return false;
			if (target.MoveType == MoveType.BeingHit && hitdef.HitFlag.ComboFlag == HitFlagCombo.No) return false;
			if (target.MoveType != MoveType.BeingHit && hitdef.HitFlag.ComboFlag == HitFlagCombo.Yes) return false;

			if (target.DefensiveInfo.HitBy1.CanHit(hitdef.HitAttribute) == false) return false;
			if (target.DefensiveInfo.HitBy2.CanHit(hitdef.HitAttribute) == false) return false;

			if (target.DefensiveInfo.IsFalling)
			{
				if (attacker is Player)
				{
					var player = attacker as Player;

#warning Juggling system is weird
					var neededjugglepoints = EvaluationHelper.AsInt32(player, player.StateManager.CurrentState.JugglePoints, 0);
					//if (neededjugglepoints > target.JugglePoints) return false;
				}
			}

			return true;
		}

		private bool CanBlock(Entity attacker, Character target, HitDefinition hitdef, bool rangecheck)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));

			if (rangecheck && Collision.HasCollision(attacker, ClsnType.Type1Attack, target, ClsnType.Type2Normal) == false) return false;

			if (hitdef.Targeting == AffectTeam.None) return false;
			if ((hitdef.Targeting & AffectTeam.Enemy) != AffectTeam.Enemy && attacker.Team != target.Team) return false;
			if ((hitdef.Targeting & AffectTeam.Friendly) != AffectTeam.Friendly && attacker.Team == target.Team) return false;

			if (target.CommandManager.IsActive("holdback") == false) return false;
			if (InGuardDistance(attacker, target, hitdef) == false) return false;

			if (target.StateType == StateType.Airborne && (hitdef.GuardFlag.HitAir == false || target.Assertions.NoAirGuard)) return false;
			if (target.StateType == StateType.Standing && (hitdef.GuardFlag.HitHigh == false || target.Assertions.NoStandingGuard)) return false;
			if (target.StateType == StateType.Crouching && (hitdef.GuardFlag.HitLow == false || target.Assertions.NoCrouchingGuard)) return false;
			if (target.StateType == StateType.Prone) return false;

			if (target.Life <= hitdef.GuardDamage && hitdef.CanGuardKill) return false;

			return true;
		}

		private bool InGuardDistance(Entity attacker, Character target, HitDefinition hitdef)
		{
			if (attacker == null) throw new ArgumentNullException(nameof(attacker));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (hitdef == null) throw new ArgumentNullException(nameof(hitdef));

			var distance = Math.Abs(attacker.CurrentLocation.X - target.CurrentLocation.X);

			return distance <= hitdef.GuardDistance;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<Contact> m_attacks;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<Contact> m_killist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Comparison<Contact> m_attacksorter;

		#endregion
	}
}