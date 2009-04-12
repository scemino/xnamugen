using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Const")]
	class Const : Function
	{
		static Const()
		{
			s_playermap = new Dictionary<String, Converter<Combat.Player, Number>>(StringComparer.OrdinalIgnoreCase);
			s_helpermap = new Dictionary<String, Converter<Combat.Helper, Number>>(StringComparer.OrdinalIgnoreCase);

			s_playermap["data.power"] = x => new Number(x.Constants.MaximumPower);
			s_helpermap["data.power"] = x => new Number(x.BasePlayer.Constants.MaximumPower);

			s_playermap["data.life"] = x => new Number(x.Constants.MaximumLife);
			s_helpermap["data.life"] = x => new Number(x.BasePlayer.Constants.MaximumLife);

			s_playermap["data.attack"] = x => new Number(x.Constants.AttackPower);
			s_helpermap["data.attack"] = x => new Number(x.BasePlayer.Constants.AttackPower);

			s_playermap["data.defence"] = x => new Number(x.Constants.DefensivePower);
			s_helpermap["data.defence"] = x => new Number(x.BasePlayer.Constants.DefensivePower);

			s_playermap["data.fall.defence_mul"] = x => new Number(100.0f / (100.0f + x.Constants.FallDefenseIncrease));
			s_helpermap["data.fall.defence_mul"] = x => new Number(100.0f / (100.0f + x.BasePlayer.Constants.FallDefenseIncrease));

			s_playermap["data.liedown.time"] = x => new Number(x.Constants.LieDownTime);
			s_helpermap["data.liedown.time"] = x => new Number(x.BasePlayer.Constants.LieDownTime);

			s_playermap["data.airjuggle"] = x => new Number(x.Constants.AirJuggle);
			s_helpermap["data.airjuggle"] = x => new Number(x.BasePlayer.Constants.AirJuggle);

			s_playermap["data.sparkno"] = x => new Number(GetDefaultHitSparkNumber(x));
			s_helpermap["data.sparkno"] = x => new Number(GetDefaultHitSparkNumber(x));

			s_playermap["data.guard.sparkno"] = x => new Number(GetDefaultGuardSparkNumber(x));
			s_helpermap["data.guard.sparkno"] = x => new Number(GetDefaultGuardSparkNumber(x));

			s_playermap["data.KO.echo"] = x => new Number(x.Constants.KOEcho);
			s_helpermap["data.KO.echo"] = x => new Number(x.BasePlayer.Constants.KOEcho);

			s_playermap["data.IntPersistIndex"] = x => new Number(x.Constants.PersistanceIntIndex);
			s_helpermap["data.IntPersistIndex"] = x => new Number(x.BasePlayer.Constants.PersistanceIntIndex);

			s_playermap["data.FloatPersistIndex"] = x => new Number(x.Constants.PersistanceFloatIndex);
			s_helpermap["data.FloatPersistIndex"] = x => new Number(x.BasePlayer.Constants.PersistanceFloatIndex);

			s_playermap["size.xscale"] = x => new Number(x.Constants.Scale.X);
			s_helpermap["size.xscale"] = x => new Number(x.Data.Scale.X);

			s_playermap["size.yscale"] = x => new Number(x.Constants.Scale.Y);
			s_helpermap["size.yscale"] = x => new Number(x.Data.Scale.Y);

			s_playermap["size.ground.back"] = x => new Number(x.Constants.GroundBack);
			s_helpermap["size.ground.back"] = x => new Number(x.BasePlayer.Constants.GroundBack);

			s_playermap["size.ground.front"] = x => new Number(x.Constants.GroundFront);
			s_helpermap["size.ground.front"] = x => new Number(x.BasePlayer.Constants.GroundFront);

			s_playermap["size.air.back"] = x => new Number(x.Constants.Airback);
			s_helpermap["size.air.back"] = x => new Number(x.BasePlayer.Constants.Airback);

			s_playermap["size.air.front"] = x => new Number(x.Constants.Airfront);
			s_helpermap["size.air.front"] = x => new Number(x.BasePlayer.Constants.Airfront);

			s_playermap["size.height"] = x => new Number(x.Constants.Height);
			s_helpermap["size.height"] = x => new Number(x.BasePlayer.Constants.Height);

			s_playermap["size.attack.dist"] = x => new Number(x.Constants.Attackdistance);
			s_helpermap["size.attack.dist"] = x => new Number(x.BasePlayer.Constants.Attackdistance);

			s_playermap["size.proj.attack.dist"] = x => new Number(x.Constants.Projectileattackdist);
			s_helpermap["size.proj.attack.dist"] = x => new Number(x.BasePlayer.Constants.Projectileattackdist);

			s_playermap["size.proj.doscale"] = x => new Number(x.Constants.ProjectileScaling);
			s_helpermap["size.proj.doscale"] = x => new Number(x.Data.ProjectileScaling);

			s_playermap["size.head.pos.x"] = x => new Number((Int32)x.Constants.Headposition.X);
			s_helpermap["size.head.pos.x"] = x => new Number((Int32)x.Data.HeadPosition.X);

			s_playermap["size.head.pos.y"] = x => new Number((Int32)x.Constants.Headposition.Y);
			s_helpermap["size.head.pos.y"] = x => new Number((Int32)x.Data.HeadPosition.Y);

			s_playermap["size.mid.pos.x"] = x => new Number((Int32)x.Constants.Midposition.X);
			s_helpermap["size.mid.pos.x"] = x => new Number((Int32)x.Data.MidPosition.X);

			s_playermap["size.mid.pos.y"] = x => new Number((Int32)x.Constants.Midposition.Y);
			s_helpermap["size.mid.pos.y"] = x => new Number((Int32)x.Data.MidPosition.Y);

			s_playermap["size.shadowoffset"] = x => new Number(x.Constants.Shadowoffset);
			s_helpermap["size.shadowoffset"] = x => new Number(x.Data.ShadowOffset);

			s_playermap["size.draw.offset.x"] = x => new Number(x.Constants.DrawOffset.X);
			s_helpermap["size.draw.offset.x"] = x => new Number(x.BasePlayer.Constants.DrawOffset.X);

			s_playermap["size.draw.offset.y"] = x => new Number(x.Constants.DrawOffset.Y);
			s_helpermap["size.draw.offset.y"] = x => new Number(x.BasePlayer.Constants.DrawOffset.Y);

			s_playermap["velocity.walk.fwd.x"] = x => new Number(x.Constants.Walk_forward);
			s_helpermap["velocity.walk.fwd.x"] = x => new Number(x.BasePlayer.Constants.Walk_forward);

			s_playermap["velocity.walk.back.x"] = x => new Number(x.Constants.Walk_back);
			s_helpermap["velocity.walk.back.x"] = x => new Number(x.BasePlayer.Constants.Walk_back);

			s_playermap["velocity.run.fwd.x"] = x => new Number(x.Constants.Run_fwd.X);
			s_helpermap["velocity.run.fwd.x"] = x => new Number(x.BasePlayer.Constants.Run_fwd.X);

			s_playermap["velocity.run.fwd.y"] = x => new Number(x.Constants.Run_fwd.Y);
			s_helpermap["velocity.run.fwd.y"] = x => new Number(x.BasePlayer.Constants.Run_fwd.Y);

			s_playermap["velocity.run.back.x"] = x => new Number(x.Constants.Run_back.X);
			s_helpermap["velocity.run.back.x"] = x => new Number(x.BasePlayer.Constants.Run_back.X);

			s_playermap["velocity.run.back.y"] = x => new Number(x.Constants.Run_back.Y);
			s_helpermap["velocity.run.back.y"] = x => new Number(x.BasePlayer.Constants.Run_back.Y);

			s_playermap["velocity.jump.y"] = x => new Number(x.Constants.Jump_neutral.Y);
			s_helpermap["velocity.jump.y"] = x => new Number(x.BasePlayer.Constants.Jump_neutral.Y);

			s_playermap["velocity.jump.neu.x"] = x => new Number(x.Constants.Jump_neutral.X);
			s_helpermap["velocity.jump.neu.x"] = x => new Number(x.BasePlayer.Constants.Jump_neutral.X);

			s_playermap["velocity.jump.back.x"] = x => new Number(x.Constants.Jump_back.X);
			s_helpermap["velocity.jump.back.x"] = x => new Number(x.BasePlayer.Constants.Jump_back.X);

			s_playermap["velocity.jump.fwd.x"] = x => new Number(x.Constants.Jump_forward.X);
			s_helpermap["velocity.jump.fwd.x"] = x => new Number(x.BasePlayer.Constants.Jump_forward.X);

			s_playermap["velocity.runjump.back.x"] = x => new Number(x.Constants.Runjump_back.X);
			s_helpermap["velocity.runjump.back.x"] = x => new Number(x.BasePlayer.Constants.Runjump_back.X);

			s_playermap["velocity.runjump.fwd.x"] = x => new Number(x.Constants.Runjump_fwd.X);
			s_helpermap["velocity.runjump.fwd.x"] = x => new Number(x.BasePlayer.Constants.Runjump_fwd.X);

			s_playermap["velocity.airjump.y"] = x => new Number(x.Constants.Airjump_neutral.Y);
			s_helpermap["velocity.airjump.y"] = x => new Number(x.BasePlayer.Constants.Airjump_neutral.Y);

			s_playermap["velocity.airjump.neu.x"] = x => new Number(x.Constants.Airjump_neutral.X);
			s_helpermap["velocity.airjump.neu.x"] = x => new Number(x.BasePlayer.Constants.Airjump_neutral.X);

			s_playermap["velocity.airjump.back.x"] = x => new Number(x.Constants.Airjump_back.X);
			s_helpermap["velocity.airjump.back.x"] = x => new Number(x.BasePlayer.Constants.Airjump_back.X);

			s_playermap["velocity.airjump.fwd.x"] = x => new Number(x.Constants.Airjump_forward.X);
			s_helpermap["velocity.airjump.fwd.x"] = x => new Number(x.BasePlayer.Constants.Airjump_forward.X);

			s_playermap["movement.airjump.num"] = x => new Number(x.Constants.Airjumps);
			s_helpermap["movement.airjump.num"] = x => new Number(x.BasePlayer.Constants.Airjumps);

			s_playermap["movement.airjump.height"] = x => new Number(x.Constants.Airjumpheight);
			s_helpermap["movement.airjump.height"] = x => new Number(x.BasePlayer.Constants.Airjumpheight);

			s_playermap["movement.yaccel"] = x => new Number(x.Constants.Vert_acceleration);
			s_helpermap["movement.yaccel"] = x => new Number(x.BasePlayer.Constants.Vert_acceleration);

			s_playermap["movement.stand.friction"] = x => new Number(x.Constants.Standfriction);
			s_helpermap["movement.stand.friction"] = x => new Number(x.BasePlayer.Constants.Standfriction);

			s_playermap["movement.crouch.friction"] = x => new Number(x.Constants.Crouchfriction);
			s_helpermap["movement.crouch.friction"] = x => new Number(x.BasePlayer.Constants.Crouchfriction);
		}

		public Const(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Arguments.Count != 1) return new Number();
			String consttype = (String)Arguments[0];

			Combat.Player player = state as Combat.Player;
			if (player != null && s_playermap.ContainsKey(consttype) == true) return s_playermap[consttype](player);

			Combat.Helper helper = state as Combat.Helper;
			if (helper != null && s_helpermap.ContainsKey(consttype) == true) return s_helpermap[consttype](helper);

			return new Number();
		}

		static Int32 GetDefaultHitSparkNumber(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			return EvaluationHelper.AsInt32(character, character.BasePlayer.Constants.DefaultSparkNumber, -1);
		}

		static Int32 GetDefaultGuardSparkNumber(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			return EvaluationHelper.AsInt32(character, character.BasePlayer.Constants.DefaultGuardSparkNumber, -1);
		}

		public static Node Parse(ParseState parsestate)
		{
			if (parsestate.CurrentSymbol != Symbol.LeftParen) return null;
			++parsestate.TokenIndex;

			String constant = parsestate.CurrentUnknown;
			if (constant == null) return null;

			parsestate.BaseNode.Arguments.Add(constant);
			++parsestate.TokenIndex;

			if (parsestate.CurrentSymbol != Symbol.RightParen) return null;
			++parsestate.TokenIndex;

			return parsestate.BaseNode;
		}

		#region Fields

		readonly static Dictionary<String, Converter<Combat.Player, Number>> s_playermap;

		readonly static Dictionary<String, Converter<Combat.Helper, Number>> s_helpermap;

		#endregion
	}
}
