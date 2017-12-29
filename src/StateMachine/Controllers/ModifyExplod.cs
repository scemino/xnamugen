using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ModifyExplod")]
	internal class ModifyExplod : StateController
	{
		public ModifyExplod(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_animationnumber = textsection.GetAttribute<Evaluation.PrefixedExpression>("anim", null);
			m_id = textsection.GetAttribute<Evaluation.Expression>("id", null);
			m_position = textsection.GetAttribute<Evaluation.Expression>("pos", null);
			m_postype = textsection.GetAttribute<PositionType?>("postype", null);
			m_facing = textsection.GetAttribute<Evaluation.Expression>("facing", null);
			m_verticalfacing = textsection.GetAttribute<Evaluation.Expression>("vfacing", null);
			m_bindtime = textsection.GetAttribute<Evaluation.Expression>("BindTime", null);

			var expVel = textsection.GetAttribute<Evaluation.Expression>("vel", null);
			var expVelocity = textsection.GetAttribute<Evaluation.Expression>("velocity", null);
			m_velocity = expVel ?? expVelocity;

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
			m_blending = textsection.GetAttribute<Blending?>("trans", null);
			m_alpha = textsection.GetAttribute<Evaluation.Expression>("alpha", null);
		}

		public override void Run(Combat.Character character)
		{
			var data = CreateModifyExplodData(character);
			if (data == null) return;

			foreach (var explod in character.GetExplods(data.Id)) explod.Modify(data);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Id == null) return false;

			return true;
		}

		private Combat.ModifyExplodData CreateModifyExplodData(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			var animationnumber = EvaluationHelper.AsInt32(character, AnimationNumber, null);
			var id = EvaluationHelper.AsInt32(character, Id, null);
			var location = EvaluationHelper.AsPoint(character, Position, null);
			var horizfacing = EvaluationHelper.AsInt32(character, Facing, null);
			var vertfacing = EvaluationHelper.AsInt32(character, VerticalFacing, null);
			var bindtime = EvaluationHelper.AsInt32(character, BindTime, null);
			var velocity = EvaluationHelper.AsVector2(character, Velocity, null);
			var acceleration = EvaluationHelper.AsVector2(character, Acceleration, null);
			var randomdisplacement = EvaluationHelper.AsPoint(character, RandomDisplacement, null);
			var removetime = EvaluationHelper.AsInt32(character, RemoveTime, null);
			var supermove = EvaluationHelper.AsBoolean(character, Supermove, null);
			var supermovetime = EvaluationHelper.AsInt32(character, SupermoveTime, null);
			var pausetime = EvaluationHelper.AsInt32(character, PauseMoveTime, null);
			var scale = EvaluationHelper.AsVector2(character, Scale, null);
			var spritepriority = EvaluationHelper.AsInt32(character, SpritePriority, null);
			var ontop = EvaluationHelper.AsBoolean(character, DrawOnTop, null);
			var ownpalette = EvaluationHelper.AsBoolean(character, OwnPalette, null);
			var removeongethit = EvaluationHelper.AsBoolean(character, RemoveOnGetHit, null);
			var ignorehitpause = EvaluationHelper.AsBoolean(character, ExplodIgnoreHitPause, null);
			var alpha = EvaluationHelper.AsPoint(character, Alpha, null);

			if (id == null) return null;

			SpriteEffects? flip = null;
			if (horizfacing != null || vertfacing != null) flip = SpriteEffects.None;

			if (horizfacing == -1) flip ^= SpriteEffects.FlipHorizontally;
			if (vertfacing == -1) flip ^= SpriteEffects.FlipVertically;

			var transparency = Transparency;
			if (transparency != null && transparency.Value.BlendType == BlendType.Add && transparency.Value.SourceFactor == 0 && transparency.Value.DestinationFactor == 0)
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

			var data = new Combat.ModifyExplodData();
			data.CommonAnimation = EvaluationHelper.IsCommon(AnimationNumber, false);
			data.AnimationNumber = animationnumber;
			data.Id = id.Value;
			data.RemoveTime = removetime;
			data.Location = (Vector2?)location;
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

		public Evaluation.PrefixedExpression AnimationNumber => m_animationnumber;

		public Evaluation.Expression Id => m_id;

		public Evaluation.Expression Position => m_position;

		public PositionType? PositionType => m_postype;

		public Evaluation.Expression Facing => m_facing;

		public Evaluation.Expression VerticalFacing => m_verticalfacing;

		public Evaluation.Expression BindTime => m_bindtime;

		public Evaluation.Expression Velocity => m_velocity;

		public Evaluation.Expression Acceleration => m_acceleration;

		public Evaluation.Expression RandomDisplacement => m_randomdisplacement;

		public Evaluation.Expression RemoveTime => m_removetime;

		public Evaluation.Expression Supermove => m_supermove;

		public Evaluation.Expression SupermoveTime => m_supermovetime;

		public Evaluation.Expression PauseMoveTime => m_pausemovetime;

		public Evaluation.Expression Scale => m_scale;

		public Evaluation.Expression SpritePriority => m_spritepriority;

		public Evaluation.Expression DrawOnTop => m_drawontop;

		public Evaluation.Expression Shadow => m_shadow;

		public Evaluation.Expression OwnPalette => m_ownpalette;

		public Evaluation.Expression RemoveOnGetHit => m_removeongethit;

		public Evaluation.Expression ExplodIgnoreHitPause => m_explodignorehitpause;

		public Blending? Transparency => m_blending;

		public Evaluation.Expression Alpha => m_alpha;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.PrefixedExpression m_animationnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_position;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PositionType? m_postype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_verticalfacing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_bindtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_velocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_acceleration;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_randomdisplacement;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_removetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_supermove;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_supermovetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_pausemovetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_scale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_spritepriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_drawontop;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_shadow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_ownpalette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_removeongethit;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_explodignorehitpause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Blending? m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_alpha;

		#endregion
	}
}
