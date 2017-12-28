using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Explod")]
	class Explod : StateController
	{
		public Explod(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_animationnumber = textsection.GetAttribute<Evaluation.PrefixedExpression>("anim", null);
			m_id = textsection.GetAttribute<Evaluation.Expression>("id", null);
			m_position = textsection.GetAttribute<Evaluation.Expression>("pos", null);
			m_postype = textsection.GetAttribute<PositionType>("postype", PositionType.P1);
			m_facing = textsection.GetAttribute<Evaluation.Expression>("facing", null);
			m_verticalfacing = textsection.GetAttribute<Evaluation.Expression>("vfacing", null);
			m_bindtime = textsection.GetAttribute<Evaluation.Expression>("BindTime", null);

			Evaluation.Expression exp_vel = textsection.GetAttribute<Evaluation.Expression>("vel", null);
			Evaluation.Expression exp_velocity = textsection.GetAttribute<Evaluation.Expression>("velocity", null);
			m_velocity = exp_vel ?? exp_velocity;

			m_acceleration = textsection.GetAttribute<Evaluation.Expression>("accel", null);
			m_randomdisplacement = textsection.GetAttribute<Evaluation.Expression>("random", null);
			m_removetime = textsection.GetAttribute<Evaluation.Expression>("removetime", null);
			m_supermove = textsection.GetAttribute<Evaluation.Expression>("supermove", null);
			m_supermovetime = textsection.GetAttribute<Evaluation.Expression>("supermovetime", null);
			m_pausemovetime = textsection.GetAttribute<Evaluation.Expression>("pausemovetime", null);
			m_scale = textsection.GetAttribute<Evaluation.Expression>("scale", null);
			m_spritepriority = textsection.GetAttribute<Evaluation.Expression>("sprpriority", null);
			m_drawontop = textsection.GetAttribute<Evaluation.Expression>("ontop", null);
			m_shadow = textsection.GetAttribute<Evaluation.Expression>("shadow", null);
			m_ownpalette = textsection.GetAttribute<Evaluation.Expression>("ownpal", null);
			m_removeongethit = textsection.GetAttribute<Evaluation.Expression>("removeongethit", null);
			m_explodignorehitpause = textsection.GetAttribute<Evaluation.Expression>("ignorehitpause", null);
			m_blending = textsection.GetAttribute<Blending>("trans", new Blending());
			m_alpha = textsection.GetAttribute<Evaluation.Expression>("alpha", null);
		}

		public override void Run(Combat.Character character)
		{
			Combat.ExplodData data = CreateExplodData(character);
			if (data == null) return;

			data.Creator = character;
			data.Offseter = (data.PositionType == PositionType.P2) ? character.GetOpponent() : character;

			Combat.Explod explod = new Combat.Explod(character.Engine, data);
			if (explod.IsValid == true)
			{
				explod.Engine.Entities.Add(explod);
			}
		}

		Combat.ExplodData CreateExplodData(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			Int32? animationnumber = EvaluationHelper.AsInt32(character, AnimationNumber, null);
			Int32 id = EvaluationHelper.AsInt32(character, Id, -1);
			Point location = EvaluationHelper.AsPoint(character, Position, new Point(0, 0));
			Int32 horizfacing = EvaluationHelper.AsInt32(character, Facing, 1);
			Int32 vertfacing = EvaluationHelper.AsInt32(character, VerticalFacing, 1);
			Int32 bindtime = EvaluationHelper.AsInt32(character, BindTime, 0);
			Vector2 velocity = EvaluationHelper.AsVector2(character, Velocity, new Vector2(0, 0));
			Vector2 acceleration = EvaluationHelper.AsVector2(character, Acceleration, new Vector2(0, 0));
			Point randomdisplacement = EvaluationHelper.AsPoint(character, RandomDisplacement, new Point(0, 0));
			Int32 removetime = EvaluationHelper.AsInt32(character, RemoveTime, -2);
			Boolean supermove = EvaluationHelper.AsBoolean(character, Supermove, false);
			Int32 supermovetime = EvaluationHelper.AsInt32(character, SupermoveTime, 0);
			Int32 pausetime = EvaluationHelper.AsInt32(character, PauseMoveTime, 0);
			Vector2 scale = EvaluationHelper.AsVector2(character, Scale, new Vector2(1, 1));
			Int32 spritepriority = EvaluationHelper.AsInt32(character, SpritePriority, 0);
			Boolean ontop = EvaluationHelper.AsBoolean(character, DrawOnTop, false);
			Boolean ownpalette = EvaluationHelper.AsBoolean(character, OwnPalette, false);
			Boolean removeongethit = EvaluationHelper.AsBoolean(character, RemoveOnGetHit, false);
			Boolean ignorehitpause = EvaluationHelper.AsBoolean(character, ExplodIgnoreHitPause, true);
			Point? alpha = EvaluationHelper.AsPoint(character, Alpha, null);

			if (animationnumber == null) return null;

			SpriteEffects flip = SpriteEffects.None;
			if (horizfacing == -1) flip ^= SpriteEffects.FlipHorizontally;
			if (vertfacing == -1) flip ^= SpriteEffects.FlipVertically;

			Blending transparency = Transparency;
			if (transparency.BlendType == BlendType.Add && transparency.SourceFactor == 0 && transparency.DestinationFactor == 0)
			{
				if (alpha != null)
				{
					transparency = new Blending(BlendType.Add, alpha.Value.X, alpha.Value.Y);
				}
				else
				{
					transparency = new Blending();
				}
			}

			Combat.ExplodData data = new Combat.ExplodData();
			data.CommonAnimation = EvaluationHelper.IsCommon(AnimationNumber, false);
			data.AnimationNumber = animationnumber.Value;
			data.Id = id;
			data.RemoveTime = removetime;
			data.Location = (Vector2)location;
			data.PositionType = PositionType;
			data.Velocity = velocity;
			data.Acceleration = acceleration;
			data.Flip = flip;
			data.BindTime = bindtime;
			data.Random = randomdisplacement;
			data.SuperMove = supermove;
			data.SuperMoveTime = supermovetime;
			data.PauseTime = pausetime;
			data.Scale = scale;
			data.SpritePriority = spritepriority;
			data.DrawOnTop = ontop;
			data.OwnPalFx = ownpalette;
			data.RemoveOnGetHit = removeongethit;
			data.IgnoreHitPause = ignorehitpause;
			data.Transparency = transparency;

			return data;
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (AnimationNumber == null) return false;

			return true;
		}

		public Evaluation.PrefixedExpression AnimationNumber
		{
			get { return m_animationnumber; }
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

		public Evaluation.Expression VerticalFacing
		{
			get { return m_verticalfacing; }
		}

		public Evaluation.Expression BindTime
		{
			get { return m_bindtime; }
		}

		public Evaluation.Expression Velocity
		{
			get { return m_velocity; }
		}

		public Evaluation.Expression Acceleration
		{
			get { return m_acceleration; }
		}

		public Evaluation.Expression RandomDisplacement
		{
			get { return m_randomdisplacement; }
		}

		public Evaluation.Expression RemoveTime
		{
			get { return m_removetime; }
		}

		public Evaluation.Expression Supermove
		{
			get { return m_supermove; }
		}

		public Evaluation.Expression SupermoveTime
		{
			get { return m_supermovetime; }
		}

		public Evaluation.Expression PauseMoveTime
		{
			get { return m_pausemovetime; }
		}

		public Evaluation.Expression Scale
		{
			get { return m_scale; }
		}

		public Evaluation.Expression SpritePriority
		{
			get { return m_spritepriority; }
		}

		public Evaluation.Expression DrawOnTop
		{
			get { return m_drawontop; }
		}

		public Evaluation.Expression Shadow
		{
			get { return m_shadow; }
		}

		public Evaluation.Expression OwnPalette
		{
			get { return m_ownpalette; }
		}

		public Evaluation.Expression RemoveOnGetHit
		{
			get { return m_removeongethit; }
		}

		public Evaluation.Expression ExplodIgnoreHitPause
		{
			get { return m_explodignorehitpause; }
		}

		public Blending Transparency
		{
			get { return m_blending; }
		}

		public Evaluation.Expression Alpha
		{
			get { return m_alpha; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.PrefixedExpression m_animationnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_position;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PositionType m_postype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_verticalfacing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_bindtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_velocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_acceleration;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_randomdisplacement;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_removetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_supermove;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_supermovetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_pausemovetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_scale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_spritepriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_drawontop;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_shadow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_ownpalette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_removeongethit;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_explodignorehitpause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_alpha;

		#endregion
	}
}
