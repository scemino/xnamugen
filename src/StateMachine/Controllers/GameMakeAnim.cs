using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("GameMakeAnim")]
	internal class GameMakeAnim : StateController
	{
		public GameMakeAnim(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_anim = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_drawunder = textsection.GetAttribute<Evaluation.Expression>("under", null);
			m_position = textsection.GetAttribute<Evaluation.Expression>("pos", null);
			m_random = textsection.GetAttribute<Evaluation.Expression>("random", null);
		}

		public override void Run(Combat.Character character)
		{
			var animationnumber = EvaluationHelper.AsInt32(character, AnimationNumber, 0);
			var drawunder = EvaluationHelper.AsBoolean(character, DrawUnder, false);
			var offset = EvaluationHelper.AsPoint(character, DrawPosition, new Point(0, 0));
			var randomdisplacement = EvaluationHelper.AsInt32(character, RandomDisplacement, 0);

			var data = new Combat.ExplodData();
			data.Scale = Vector2.One;
			data.AnimationNumber = animationnumber;
			data.CommonAnimation = true;
			data.Location = (Vector2)offset;
			data.PositionType = PositionType.P1;
			data.RemoveTime = -2;
			data.DrawOnTop = false;
			data.OwnPalFx = true;
			data.SpritePriority = drawunder ? -9 : 9;
			data.Random = new Point(randomdisplacement / 2, randomdisplacement / 2);
			data.Transparency = new Blending();
			data.Creator = character;
			data.Offseter = character;

			var explod = new Combat.Explod(character.Engine, data);
			if (explod.IsValid) explod.Engine.Entities.Add(explod);
		}

		public Evaluation.Expression AnimationNumber => m_anim;

		public Evaluation.Expression DrawUnder => m_drawunder;

		public Evaluation.Expression DrawPosition => m_position;

		public Evaluation.Expression RandomDisplacement => m_random;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Evaluation.Expression m_anim;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Evaluation.Expression m_drawunder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Evaluation.Expression m_position;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Evaluation.Expression m_random;

		#endregion
	}
}