using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("BindToTarget")]
	internal class BindToTarget : StateController
	{
		public BindToTarget(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("id", null);

			ParsePositionString(textsection.GetAttribute<string>("pos", null), out m_position, out m_bindpos);
		}

		private void ParsePositionString(string input, out Evaluation.Expression expression, out BindToTargetPostion postype)
		{
			expression = null;
			postype = BindToTargetPostion.None;

			if (input == null) return;

			var sepIndex = input.LastIndexOf(',');
			if (sepIndex == -1)
			{
				expression = StateSystem.GetSubSystem<Evaluation.EvaluationSystem>().CreateExpression(input);
				return;
			}

			var strExp = input.Substring(0, sepIndex).Trim();
			var strPostype = input.Substring(sepIndex + 1).Trim();

			if (StateSystem.GetSubSystem<StringConverter>().TryConvert(strPostype, out BindToTargetPostion bttp))
			{
				expression = StateSystem.GetSubSystem<Evaluation.EvaluationSystem>().CreateExpression(strExp);
				postype = bttp;
			}
			else
			{
				expression = StateSystem.GetSubSystem<Evaluation.EvaluationSystem>().CreateExpression(input);
			}
		}

		public override void Run(Combat.Character character)
		{
			var time = EvaluationHelper.AsInt32(character, Time, 1);
			var targetId = EvaluationHelper.AsInt32(character, TargetId, int.MinValue);
			var offset = EvaluationHelper.AsVector2(character, Position, new Vector2(0, 0));

			foreach (var target in character.GetTargets(targetId))
			{
				switch (BindPosition)
				{
					case BindToTargetPostion.Mid:
						offset += target.BasePlayer.Constants.Midposition;
						break;

					case BindToTargetPostion.Head:
						offset += target.BasePlayer.Constants.Headposition;
						break;
				}

				character.Bind.Set(target, offset, time, 0, false);
				break;
			}
		}

		public Evaluation.Expression Time => m_time;

		public Evaluation.Expression TargetId => m_targetid;

		public Evaluation.Expression Position => m_position;

		public BindToTargetPostion BindPosition => m_bindpos;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_targetid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly BindToTargetPostion m_bindpos;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_position;

		#endregion
	}
}