namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Const")]
	internal static class Const
	{
		[Tag("data.life")]
		public static int Data_Life(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.MaximumLife;
		}

		[Tag("data.attack")]
		public static int Data_Attack(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.AttackPower;
		}

		[Tag("data.defence")]
		public static int Data_Defence(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.DefensivePower;
		}

		[Tag("data.fall.defence_mul")]
		public static float Data_Fall_Defence_Mul(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return 100.0f / (100.0f + character.BasePlayer.Constants.FallDefenseIncrease);
		}

		[Tag("data.liedown.time")]
		public static int Data_Liedown_Time(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.LieDownTime;
		}

		[Tag("data.airjuggle")]
		public static int Data_Airjuggle(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.AirJuggle;
		}

		[Tag("data.sparkno")]
		public static int Data_Sparkno(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return EvaluationHelper.AsInt32(character, character.BasePlayer.Constants.DefaultSparkNumber, -1);
		}

		[Tag("data.guard.sparkno")]
		public static int Data_Guard_Sparkno(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return EvaluationHelper.AsInt32(character, character.BasePlayer.Constants.DefaultGuardSparkNumber, -1);
		}

		[Tag("data.KO.echo")]
		public static bool Data_Ko_Echo(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.BasePlayer.Constants.KOEcho;
		}

		[Tag("data.IntPersistIndex")]
		public static int Data_IntPersistIndex(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.PersistanceIntIndex;
		}

		[Tag("data.FloatPersistIndex")]
		public static int Data_FloatPersistIndex(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.PersistanceFloatIndex;
		}

		[Tag("size.draw.offset.x")]
		public static int Size_Draw_Offset_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Drawoffset.X;
		}

		[Tag("size.draw.offset.y")]
		public static int Size_Draw_Offset_Y(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Drawoffset.Y;
		}

		[Tag("size.xscale")]
		public static float Size_Xscale(object state, ref bool error)
		{
			var player = state as Combat.Player;
			if (player != null) return player.Constants.Scale.X;

			var helper = state as Combat.Helper;
			if (helper != null) return helper.Data.Scale.X;

			error = true;
			return 0;
		}

		[Tag("size.yscale")]
		public static float Size_Yscale(object state, ref bool error)
		{
			var player = state as Combat.Player;
			if (player != null) return player.Constants.Scale.Y;

			var helper = state as Combat.Helper;
			if (helper != null) return helper.Data.Scale.Y;

			error = true;
			return 0;
		}

		[Tag("size.ground.back")]
		public static int Size_Ground_Back(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.GroundBack;
		}

		[Tag("size.ground.front")]
		public static int Size_Ground_Front(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.GroundFront;
		}

		[Tag("Size.Air.Back")]
		public static int Size_Air_Back(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airback;
		}

		[Tag("Size.Air.Front")]
		public static int Size_Air_Front(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airfront;
		}

		[Tag("Size.Height")]
		public static int Size_Height(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Height;
		}

		[Tag("Size.Attack.Dist")]
		public static int Size_Attack_Dist(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Attackdistance;
		}

		[Tag("Size.Proj.Attack.Dist")]
		public static int Size_Proj_Attack_Dist(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Projectileattackdist;
		}

		[Tag("Size.Proj.Doscale")]
		public static bool Size_Proj_Doscale(object state, ref bool error)
		{
			var player = state as Combat.Player;
			if (player != null) return player.Constants.ProjectileScaling;

			var helper = state as Combat.Helper;
			if (helper != null) return helper.Data.ProjectileScaling;

			error = true;
			return false;
		}

		[Tag("Size.Head.Pos.X")]
		public static float Size_Head_Pos_X(object state, ref bool error)
		{
			var player = state as Combat.Player;
			if (player != null) return player.Constants.Headposition.X;

			var helper = state as Combat.Helper;
			if (helper != null) return helper.Data.HeadPosition.X;

			error = true;
			return 0;
		}

		[Tag("Size.Head.Pos.Y")]
		public static float Size_Head_Pos_Y(object state, ref bool error)
		{
			var player = state as Combat.Player;
			if (player != null) return player.Constants.Headposition.Y;

			var helper = state as Combat.Helper;
			if (helper != null) return helper.Data.HeadPosition.Y;

			error = true;
			return 0;
		}

		[Tag("size.mid.pos.x")]
		public static float Size_Mid_Pos_X(object state, ref bool error)
		{
			var player = state as Combat.Player;
			if (player != null) return player.Constants.Midposition.X;

			var helper = state as Combat.Helper;
			if (helper != null) return helper.Data.MidPosition.X;

			error = true;
			return 0;
		}

		[Tag("size.mid.pos.y")]
		public static float Size_Mid_Pos_Y(object state, ref bool error)
		{
			var player = state as Combat.Player;
			if (player != null) return player.Constants.Midposition.Y;

			var helper = state as Combat.Helper;
			if (helper != null) return helper.Data.MidPosition.Y;

			error = true;
			return 0;
		}

		[Tag("Size.Shadowoffset")]
		public static int Size_Shadowoffset(object state, ref bool error)
		{
			var player = state as Combat.Player;
			if (player != null) return player.Constants.Shadowoffset;

			var helper = state as Combat.Helper;
			if (helper != null) return helper.Data.ShadowOffset;

			error = true;
			return 0;
		}

		[Tag("Velocity.Walk.Fwd.X")]
		public static float Velocity_Walk_Fwd_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Walk_forward;
		}

		[Tag("Velocity.Walk.Back.X")]
		public static float Velocity_Walk_Back_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Walk_back;
		}

		[Tag("Velocity.Run.Fwd.X")]
		public static float Velocity_Run_Fwd_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Run_fwd.X;
		}

		[Tag("Velocity.Run.Fwd.Y")]
		public static float Velocity_Run_Fwd_Y(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Run_fwd.Y;
		}

		[Tag("Velocity.Run.Back.X")]
		public static float Velocity_Run_Back_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Run_back.X;
		}

		[Tag("Velocity.Run.Back.Y")]
		public static float Velocity_Run_Back_Y(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Run_back.Y;
		}

		[Tag("Velocity.Jump.Y")]
		public static float Velocity_Jump_Y(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Jump_neutral.Y;
		}

		[Tag("Velocity.Jump.Neu.X")]
		public static float Velocity_Jump_Neu_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Jump_neutral.X;
		}

		[Tag("Velocity.Jump.Back.X")]
		public static float Velocity_Jump_Back_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Jump_back.X;
		}

		[Tag("Velocity.Jump.Fwd.X")]
		public static float Velocity_Jump_Fwd_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Jump_forward.X;
		}

		[Tag("Velocity.Runjump.Back.X")]
		public static float Velocity_Runjump_Back_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Runjump_back.X;
		}

		[Tag("Velocity.Runjump.Fwd.X")]
		public static float Velocity_Runjump_Fwd_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Runjump_fwd.X;
		}

		[Tag("Velocity.Airjump.Y")]
		public static float Velocity_Airjump_Y(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjump_neutral.Y;
		}

		[Tag("Velocity.Airjump.Neu.X")]
		public static float Velocity_Airjump_Neu_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjump_neutral.X;
		}

		[Tag("Velocity.Airjump.Back.X")]
		public static float Velocity_Airjump_Back_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjump_back.X;
		}

		[Tag("Velocity.Airjump.Fwd.X")]
		public static float Velocity_Airjump_Fwd_X(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjump_forward.X;
		}

		[Tag("Movement.Airjump.Num")]
		public static int Movement_Airjump_Num(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjumps;
		}

		[Tag("Movement.Airjump.Height")]
		public static int Movement_Airjump_Height(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Airjumpheight;
		}

		[Tag("Movement.Yaccel")]
		public static float Movement_Yaccel(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Vert_acceleration;
		}

		[Tag("Movement.Stand.Friction")]
		public static float Movement_Stand_Friction(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.Constants.Standfriction;
		}

		[Tag("Movement.Crouch.Friction")]
		public static float Movement_Crouch_Friction(object state, ref bool error)
		{
			var character = state as Combat.Character;
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

			var constant = state.CurrentUnknown;
			if (constant == null) return null;

			state.BaseNode.Arguments.Add(constant);
			++state.TokenIndex;

			if (state.CurrentSymbol != Symbol.RightParen) return null;
			++state.TokenIndex;

			return state.BaseNode;
		}
	}
}
