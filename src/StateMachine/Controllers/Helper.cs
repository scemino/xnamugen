using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Helper")]
	class Helper : StateController
	{
		public Helper(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_helpertype = textsection.GetAttribute<HelperType>("helpertype", HelperType.Normal);
			m_name = textsection.GetAttribute<String>("name", null);
			m_id = textsection.GetAttribute<Evaluation.Expression>("id", null);
			m_position = textsection.GetAttribute<Evaluation.Expression>("pos", null);
			m_postype = textsection.GetAttribute<PositionType>("postype", PositionType.P1);
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
			String helper_name = Name ?? character.BasePlayer.Profile.DisplayName + "'s Helper";
			Int32 helper_id = EvaluationHelper.AsInt32(character, Id, 0);
			Vector2 position_offset = (Vector2)EvaluationHelper.AsPoint(character, Position, new Point(0, 0));
			Int32 facingflag = EvaluationHelper.AsInt32(character, Facing, 1);
			Int32 statenumber = EvaluationHelper.AsInt32(character, StateNumber, 0);
			Boolean keycontrol = EvaluationHelper.AsBoolean(character, KeyControl, false);
			Boolean ownpalette = EvaluationHelper.AsBoolean(character, OwnPalette, false);
			Int32 supermovetime = EvaluationHelper.AsInt32(character, SuperMoveTime, 0);
			Int32 pausemovetime = EvaluationHelper.AsInt32(character, PauseMoveTime, 0);
			Single scalex = EvaluationHelper.AsSingle(character, XScale, character.BasePlayer.Constants.Scale.X);
			Single scaley = EvaluationHelper.AsSingle(character, YScale, character.BasePlayer.Constants.Scale.Y);
			Int32 groundfront = EvaluationHelper.AsInt32(character, GroundFrontSize, character.BasePlayer.Constants.GroundFront);
			Int32 groundback = EvaluationHelper.AsInt32(character, GroundBackSize, character.BasePlayer.Constants.GroundBack);
			Int32 airfront = EvaluationHelper.AsInt32(character, AirFrontSize, character.BasePlayer.Constants.Airfront);
			Int32 airback = EvaluationHelper.AsInt32(character, AirBackSize, character.BasePlayer.Constants.Airback);
			Int32 height = EvaluationHelper.AsInt32(character, Height, character.BasePlayer.Constants.Height);
			Boolean projectilescaling = EvaluationHelper.AsBoolean(character, ProjectileScaling, character.BasePlayer.Constants.ProjectileScaling);
			Vector2 headposition = (Vector2)EvaluationHelper.AsPoint(character, HeadPosition, (Point)character.BasePlayer.Constants.Headposition);
			Vector2 midposition = (Vector2)EvaluationHelper.AsPoint(character, MiddlePosition, (Point)character.BasePlayer.Constants.Midposition);
			Int32 shadowoffset = EvaluationHelper.AsInt32(character, ShadowOffset, character.BasePlayer.Constants.ShadowOffset);

			Combat.HelperData data = new Combat.HelperData();
			data.Name = helper_name;
			data.HelperId = helper_id;
			data.Type = HelperType;
			data.FacingFlag = facingflag;
			data.PositionType = PositionType;
			data.CreationOffset = position_offset;
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

			Combat.Helper helper = new Combat.Helper(character.Engine, character, data);
			helper.Engine.Entities.Add(helper);
		}

		public HelperType HelperType
		{
			get { return m_helpertype; }
		}

		public String Name
		{
			get { return m_name; }
		}

		public Evaluation.Expression Id
		{
			get { return m_id; }
		}

		public Evaluation.Expression Position
		{
			get { return m_position; }
		}

		public PositionType PositionType
		{
			get { return m_postype; }
		}

		public Evaluation.Expression Facing
		{
			get { return m_facing; }
		}

		public Evaluation.Expression StateNumber
		{
			get { return m_statenumber; }
		}

		public Evaluation.Expression KeyControl
		{
			get { return m_keyctrl; }
		}

		public Evaluation.Expression OwnPalette
		{
			get { return m_ownpal; }
		}

		public Evaluation.Expression SuperMoveTime
		{
			get { return m_supermovetime; }
		}

		public Evaluation.Expression PauseMoveTime
		{
			get { return m_pausemovetime; }
		}

		public Evaluation.Expression XScale
		{
			get { return m_xscale; }
		}

		public Evaluation.Expression YScale
		{
			get { return m_yscale; }
		}

		public Evaluation.Expression GroundBackSize
		{
			get { return m_groundback; }
		}

		public Evaluation.Expression GroundFrontSize
		{
			get { return m_groundfront; }
		}

		public Evaluation.Expression AirBackSize
		{
			get { return m_airback; }
		}

		public Evaluation.Expression AirFrontSize
		{
			get { return m_airfront; }
		}

		public Evaluation.Expression Height
		{
			get { return m_height; }
		}

		public Evaluation.Expression ProjectileScaling
		{
			get { return m_projectscaling; }
		}

		public Evaluation.Expression HeadPosition
		{
			get { return m_headpos; }
		}

		public Evaluation.Expression MiddlePosition
		{
			get { return m_midpos; }
		}

		public Evaluation.Expression ShadowOffset
		{
			get { return m_shadowoffset; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly HelperType m_helpertype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_position;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PositionType m_postype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_keyctrl;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_ownpal;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_supermovetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_pausemovetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_xscale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_yscale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_groundback;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_groundfront;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_airback;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_airfront;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_height;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_projectscaling;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_headpos;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_midpos;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_shadowoffset;

		#endregion

	}
}