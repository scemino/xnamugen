using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("GameMakeAnim")]
	class GameMakeAnim : StateController
	{
		public GameMakeAnim(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_anim = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_drawunder = textsection.GetAttribute<Evaluation.Expression>("under", null);
			m_position = textsection.GetAttribute<Evaluation.Expression>("pos", null);
			m_random = textsection.GetAttribute<Evaluation.Expression>("random", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32 animationnumber = EvaluationHelper.AsInt32(character, AnimationNumber, 0);
			Boolean drawunder = EvaluationHelper.AsBoolean(character, DrawUnder, false);
			Point offset = EvaluationHelper.AsPoint(character, DrawPosition, new Point(0, 0));
			Int32 randomdisplacement = EvaluationHelper.AsInt32(character, RandomDisplacement, 0);

			Combat.ExplodData data = new xnaMugen.Combat.ExplodData();
			data.Scale = Vector2.One;
			data.AnimationNumber = animationnumber;
			data.CommonAnimation = true;
			data.Location = (Vector2)offset;
			data.PositionType = PositionType.P1;
			data.RemoveTime = -2;
			data.DrawOnTop = false;
			data.OwnPalFx = true;
			data.SpritePriority = (drawunder == true) ? -9 : 9;
			data.Random = new Point(randomdisplacement / 2, randomdisplacement / 2);
			data.Transparency = new Blending();
			data.Creator = character;
			data.Offseter = character;

			Combat.Explod explod = new Combat.Explod(character.Engine, data);
			if (explod.IsValid == true) explod.Engine.Entities.Add(explod);
		}

		public Evaluation.Expression AnimationNumber
		{
			get { return m_anim; }
		}

		public Evaluation.Expression DrawUnder
		{
			get { return m_drawunder; }
		}

		public Evaluation.Expression DrawPosition
		{
			get { return m_position; }
		}

		public Evaluation.Expression RandomDisplacement
		{
			get { return m_random; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Evaluation.Expression m_anim;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Evaluation.Expression m_drawunder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Evaluation.Expression m_position;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Evaluation.Expression m_random;

		#endregion
	}
}