using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Const")]
	static class Const
	{
		[Tag("data.life")]
		public static Int32 Data_Life(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.MaximumLife;
		}

		[Tag("data.attack")]
		public static Int32 Data_Attack(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.AttackPower;
		}

		[Tag("data.defence")]
		public static Int32 Data_Defence(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.DefensivePower;
		}

		[Tag("data.fall.defence_mul")]
		public static Single Data_Fall_Defence_Mul(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return 100.0f / (100.0f + character.BasePlayer.Constants.FallDefenseIncrease);
		}

		[Tag("data.liedown.time")]
		public static Int32 Data_Liedown_Time(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.LieDownTime;
		}

		[Tag("data.airjuggle")]
		public static Int32 Data_Airjuggle(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.AirJuggle;
		}

		[Tag("data.sparkno")]
		public static Int32 Data_Sparkno(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return EvaluationHelper.AsInt32(character, character.BasePlayer.Constants.DefaultSparkNumber, -1);
		}

		[Tag("data.guard.sparkno")]
		public static Int32 Data_Guard_Sparkno(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return EvaluationHelper.AsInt32(character, character.BasePlayer.Constants.DefaultGuardSparkNumber, -1);
		}

		[Tag("data.KO.echo")]
		public static Boolean Data_Ko_Echo(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.BasePlayer.Constants.KOEcho;
		}

		[Tag("data.IntPersistIndex")]
		public static Int32 Data_IntPersistIndex(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.PersistanceIntIndex;
		}

		[Tag("data.FloatPersistIndex")]
		public static Int32 Data_FloatPersistIndex(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.PersistanceFloatIndex;
		}

		[Tag("size.draw.offset.x")]
		public static Int32 Size_Draw_Offset_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Drawoffset.X;
		}

		[Tag("size.draw.offset.y")]
		public static Int32 Size_Draw_Offset_Y(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Drawoffset.Y;
		}

		[Tag("size.xscale")]
		public static Single Size_Xscale(Object state, ref Boolean error)
		{
			Combat.Player player = state as Combat.Player;
			if (player != null) return player.Constants.Scale.X;

			Combat.Helper helper = state as Combat.Helper;
			if (helper != null) return helper.Data.Scale.X;

			error = true;
			return 0;
		}

		[Tag("size.yscale")]
		public static Single Size_Yscale(Object state, ref Boolean error)
		{
			Combat.Player player = state as Combat.Player;
			if (player != null) return player.Constants.Scale.Y;

			Combat.Helper helper = state as Combat.Helper;
			if (helper != null) return helper.Data.Scale.Y;

			error = true;
			return 0;
		}

		[Tag("size.ground.back")]
		public static Int32 Size_Ground_Back(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.GroundBack;
		}

		[Tag("size.ground.front")]
		public static Int32 Size_Ground_Front(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.GroundFront;
		}

		[Tag("Size.Air.Back")]
		public static Int32 Size_Air_Back(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airback;
		}

		[Tag("Size.Air.Front")]
		public static Int32 Size_Air_Front(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airfront;
		}

		[Tag("Size.Height")]
		public static Int32 Size_Height(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Height;
		}

		[Tag("Size.Attack.Dist")]
		public static Int32 Size_Attack_Dist(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Attackdistance;
		}

		[Tag("Size.Proj.Attack.Dist")]
		public static Int32 Size_Proj_Attack_Dist(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Projectileattackdist;
		}

		[Tag("Size.Proj.Doscale")]
		public static Boolean Size_Proj_Doscale(Object state, ref Boolean error)
		{
			Combat.Player player = state as Combat.Player;
			if (player != null) return player.Constants.ProjectileScaling;

			Combat.Helper helper = state as Combat.Helper;
			if (helper != null) return helper.Data.ProjectileScaling;

			error = true;
			return false;
		}

		[Tag("Size.Head.Pos.X")]
		public static Single Size_Head_Pos_X(Object state, ref Boolean error)
		{
			Combat.Player player = state as Combat.Player;
			if (player != null) return player.Constants.Headposition.X;

			Combat.Helper helper = state as Combat.Helper;
			if (helper != null) return helper.Data.HeadPosition.X;

			error = true;
			return 0;
		}

		[Tag("Size.Head.Pos.Y")]
		public static Single Size_Head_Pos_Y(Object state, ref Boolean error)
		{
			Combat.Player player = state as Combat.Player;
			if (player != null) return player.Constants.Headposition.Y;

			Combat.Helper helper = state as Combat.Helper;
			if (helper != null) return helper.Data.HeadPosition.Y;

			error = true;
			return 0;
		}

		[Tag("size.mid.pos.x")]
		public static Single Size_Mid_Pos_X(Object state, ref Boolean error)
		{
			Combat.Player player = state as Combat.Player;
			if (player != null) return player.Constants.Midposition.X;

			Combat.Helper helper = state as Combat.Helper;
			if (helper != null) return helper.Data.MidPosition.X;

			error = true;
			return 0;
		}

		[Tag("size.mid.pos.y")]
		public static Single Size_Mid_Pos_Y(Object state, ref Boolean error)
		{
			Combat.Player player = state as Combat.Player;
			if (player != null) return player.Constants.Midposition.Y;

			Combat.Helper helper = state as Combat.Helper;
			if (helper != null) return helper.Data.MidPosition.Y;

			error = true;
			return 0;
		}

		[Tag("Size.Shadowoffset")]
		public static Int32 Size_Shadowoffset(Object state, ref Boolean error)
		{
			Combat.Player player = state as Combat.Player;
			if (player != null) return player.Constants.Shadowoffset;

			Combat.Helper helper = state as Combat.Helper;
			if (helper != null) return helper.Data.ShadowOffset;

			error = true;
			return 0;
		}

		[Tag("Velocity.Walk.Fwd.X")]
		public static Single Velocity_Walk_Fwd_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Walk_forward;
		}

		[Tag("Velocity.Walk.Back.X")]
		public static Single Velocity_Walk_Back_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Walk_back;
		}

		[Tag("Velocity.Run.Fwd.X")]
		public static Single Velocity_Run_Fwd_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Run_fwd.X;
		}

		[Tag("Velocity.Run.Fwd.Y")]
		public static Single Velocity_Run_Fwd_Y(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Run_fwd.Y;
		}

		[Tag("Velocity.Run.Back.X")]
		public static Single Velocity_Run_Back_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Run_back.X;
		}

		[Tag("Velocity.Run.Back.Y")]
		public static Single Velocity_Run_Back_Y(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Run_back.Y;
		}

		[Tag("Velocity.Jump.Y")]
		public static Single Velocity_Jump_Y(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Jump_neutral.Y;
		}

		[Tag("Velocity.Jump.Neu.X")]
		public static Single Velocity_Jump_Neu_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Jump_neutral.X;
		}

		[Tag("Velocity.Jump.Back.X")]
		public static Single Velocity_Jump_Back_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Jump_back.X;
		}

		[Tag("Velocity.Jump.Fwd.X")]
		public static Single Velocity_Jump_Fwd_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Jump_forward.X;
		}

		[Tag("Velocity.Runjump.Back.X")]
		public static Single Velocity_Runjump_Back_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Runjump_back.X;
		}

		[Tag("Velocity.Runjump.Fwd.X")]
		public static Single Velocity_Runjump_Fwd_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Runjump_fwd.X;
		}

		[Tag("Velocity.Airjump.Y")]
		public static Single Velocity_Airjump_Y(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjump_neutral.Y;
		}

		[Tag("Velocity.Airjump.Neu.X")]
		public static Single Velocity_Airjump_Neu_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjump_neutral.X;
		}

		[Tag("Velocity.Airjump.Back.X")]
		public static Single Velocity_Airjump_Back_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjump_back.X;
		}

		[Tag("Velocity.Airjump.Fwd.X")]
		public static Single Velocity_Airjump_Fwd_X(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjump_forward.X;
		}

		[Tag("Movement.Airjump.Num")]
		public static Int32 Movement_Airjump_Num(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjumps;
		}

		[Tag("Movement.Airjump.Height")]
		public static Int32 Movement_Airjump_Height(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjumpheight;
		}

		[Tag("Movement.Yaccel")]
		public static Single Movement_Yaccel(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Vert_acceleration;
		}

		[Tag("Movement.Stand.Friction")]
		public static Single Movement_Stand_Friction(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Standfriction;
		}

		[Tag("Movement.Crouch.Friction")]
		public static Single Movement_Crouch_Friction(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Crouchfriction;
		}

		public static Node Parse(ParseState state)
		{
			if (state.CurrentSymbol != Symbol.LeftParen) return null;
			++state.TokenIndex;

			String constant = state.CurrentUnknown;
			if (constant == null) return null;

			state.BaseNode.Arguments.Add(constant);
			++state.TokenIndex;

			if (state.CurrentSymbol != Symbol.RightParen) return null;
			++state.TokenIndex;

			return state.BaseNode;
		}
	}
}
