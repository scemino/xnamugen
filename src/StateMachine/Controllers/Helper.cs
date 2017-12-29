using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Helper")]
	internal class Helper : StateController
	{
		public Helper(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_helpertype = textsection.GetAttribute("helpertype", HelperType.Normal);
			m_name = textsection.GetAttribute<string>("name", null);
			m_id = textsection.GetAttribute<Evaluation.Expression>("id", null);
			m_position = textsection.GetAttribute<Evaluation.Expression>("pos", null);
			m_postype = textsection.GetAttribute("postype", PositionType.P1);
			m_facing = textsection.GetAttribute<Evaluation.Expression>("facing", null);
			m_statenumber = textsection.GetAttribute<Evaluation.Expression>("stateno", null);
			m_keyctrl = textsection.GetAttribute<Evaluation.Expression>("keyctrl", null);
			m_ownpal = textsection.GetAttribute<Evaluation.Expression>("ownpal", null);
			m_supermovetime = textsection.GetAttribute<Evaluation.Expression>("SuperMoveTime", null);
			m_pausemovetime = textsection.GetAttribute<Evaluation.Expression>("PauseMoveTime", null);
			m_xscale = textsection.GetAttribute<Evaluation.Expression>("size.xscale", null);
			m_yscale = textsection.GetAttribute<Evaluation.Expression>("size.yscale", null);
			m_groundback = textsection.GetAttribute<Evaluation.Expression>("size.ground.back", null);
			m_groundfront = textsection.GetAttribute<Evaluation.Expression>("size.ground.front", null);
			m_airback = textsection.GetAttribute<Evaluation.Expression>("size.air.back", null);
			m_airfront = textsection.GetAttribute<Evaluation.Expression>("size.air.front", null);
			m_height = textsection.GetAttribute<Evaluation.Expression>("size.height", null);
			m_projectscaling = textsection.GetAttribute<Evaluation.Expression>("size.proj.doscale", null);
			m_headpos = textsection.GetAttribute<Evaluation.Expression>("size.head.pos", null);
			m_midpos = textsection.GetAttribute<Evaluation.Expression>("size.mid.pos", null);
			m_shadowoffset = textsection.GetAttribute<Evaluation.Expression>("size.shadowoffset", null);
		}

		public override void Run(Combat.Character character)
		{
			var helperName = Name ?? character.BasePlayer.Profile.DisplayName + "'s Helper";
			var helperId = EvaluationHelper.AsInt32(character, Id, 0);
			var positionOffset = (Vector2)EvaluationHelper.AsPoint(character, Position, new Point(0, 0));
			var facingflag = EvaluationHelper.AsInt32(character, Facing, 1);
			var statenumber = EvaluationHelper.AsInt32(character, StateNumber, 0);
			var keycontrol = EvaluationHelper.AsBoolean(character, KeyControl, false);
			var ownpalette = EvaluationHelper.AsBoolean(character, OwnPalette, false);
			var supermovetime = EvaluationHelper.AsInt32(character, SuperMoveTime, 0);
			var pausemovetime = EvaluationHelper.AsInt32(character, PauseMoveTime, 0);
			var scalex = EvaluationHelper.AsSingle(character, XScale, character.BasePlayer.Constants.Scale.X);
			var scaley = EvaluationHelper.AsSingle(character, YScale, character.BasePlayer.Constants.Scale.Y);
			var groundfront = EvaluationHelper.AsInt32(character, GroundFrontSize, character.BasePlayer.Constants.GroundFront);
			var groundback = EvaluationHelper.AsInt32(character, GroundBackSize, character.BasePlayer.Constants.GroundBack);
			var airfront = EvaluationHelper.AsInt32(character, AirFrontSize, character.BasePlayer.Constants.Airfront);
			var airback = EvaluationHelper.AsInt32(character, AirBackSize, character.BasePlayer.Constants.Airback);
			var height = EvaluationHelper.AsInt32(character, Height, character.BasePlayer.Constants.Height);
			var projectilescaling = EvaluationHelper.AsBoolean(character, ProjectileScaling, character.BasePlayer.Constants.ProjectileScaling);
			var headposition = (Vector2)EvaluationHelper.AsPoint(character, HeadPosition, (Point)character.BasePlayer.Constants.Headposition);
			var midposition = (Vector2)EvaluationHelper.AsPoint(character, MiddlePosition, (Point)character.BasePlayer.Constants.Midposition);
			var shadowoffset = EvaluationHelper.AsInt32(character, ShadowOffset, character.BasePlayer.Constants.Shadowoffset);

			var data = new Combat.HelperData();
			data.Name = helperName;
			data.HelperId = helperId;
			data.Type = HelperType;
			data.FacingFlag = facingflag;
			data.PositionType = PositionType;
			data.CreationOffset = positionOffset;
			data.KeyControl = keycontrol;
			data.OwnPaletteFx = ownpalette;
			data.InitialStateNumber = statenumber;
			data.Scale = new Vector2(scalex, scaley);
			data.GroundFront = groundfront;
			data.GroundBack = groundback;
			data.AirFront = airfront;
			data.AirBack = airback;
			data.Height = height;
			data.SuperPauseTime = supermovetime;
			data.PauseTime = pausemovetime;
			data.ProjectileScaling = projectilescaling;
			data.HeadPosition = headposition;
			data.MidPosition = midposition;
			data.ShadowOffset = shadowoffset;

			var helper = new Combat.Helper(character.Engine, character, data);
			helper.Engine.Entities.Add(helper);
		}

		public HelperType HelperType => m_helpertype;

		public string Name => m_name;

		public Evaluation.Expression Id => m_id;

		public Evaluation.Expression Position => m_position;

		public PositionType PositionType => m_postype;

		public Evaluation.Expression Facing => m_facing;

		public Evaluation.Expression StateNumber => m_statenumber;

		public Evaluation.Expression KeyControl => m_keyctrl;

		public Evaluation.Expression OwnPalette => m_ownpal;

		public Evaluation.Expression SuperMoveTime => m_supermovetime;

		public Evaluation.Expression PauseMoveTime => m_pausemovetime;

		public Evaluation.Expression XScale => m_xscale;

		public Evaluation.Expression YScale => m_yscale;

		public Evaluation.Expression GroundBackSize => m_groundback;

		public Evaluation.Expression GroundFrontSize => m_groundfront;

		public Evaluation.Expression AirBackSize => m_airback;

		public Evaluation.Expression AirFrontSize => m_airfront;

		public Evaluation.Expression Height => m_height;

		public Evaluation.Expression ProjectileScaling => m_projectscaling;

		public Evaluation.Expression HeadPosition => m_headpos;

		public Evaluation.Expression MiddlePosition => m_midpos;

		public Evaluation.Expression ShadowOffset => m_shadowoffset;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly HelperType m_helpertype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_position;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PositionType m_postype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_keyctrl;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_ownpal;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_supermovetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_pausemovetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_xscale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_yscale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_groundback;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_groundfront;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_airback;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_airfront;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_height;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_projectscaling;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_headpos;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_midpos;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_shadowoffset;

		#endregion

	}
}