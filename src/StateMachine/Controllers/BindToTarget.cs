using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("BindToTarget")]
	class BindToTarget : StateController
	{
		public BindToTarget(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("id", null);

			ParsePositionString(textsection.GetAttribute<String>("pos", null), out m_position, out m_bindpos);
		}

		void ParsePositionString(String input, out Evaluation.Expression expression, out BindToTargetPostion postype)
		{
			expression = null;
			postype = BindToTargetPostion.None;

			if (input == null) return;

			Int32 sep_index = input.LastIndexOf(',');
			if (sep_index == -1)
			{
				expression = StateSystem.GetSubSystem<Evaluation.EvaluationSystem>().CreateExpression(input);
				return;
			}

			StringSubString postype_str = new StringSubString(input, sep_index + 1, input.Length);
			postype_str.TrimWhitespace();

			BindToTargetPostion bttp;
			if (StateSystem.GetSubSystem<StringConverter>().TryConvert(postype_str.ToString(), out bttp) == true)
			{
				StringSubString exp_str = new StringSubString(input, 0, sep_index);
				exp_str.TrimWhitespace();

				expression = StateSystem.GetSubSystem<Evaluation.EvaluationSystem>().CreateExpression(exp_str.ToString());
				postype = bttp;
			}
			else
			{
				expression = StateSystem.GetSubSystem<Evaluation.EvaluationSystem>().CreateExpression(input);
			}
		}

		public override void Run(Combat.Character character)
		{
			Int32 time = EvaluationHelper.AsInt32(character, Time, 1);
			Int32? target_id = EvaluationHelper.AsInt32(character, TargetId, null);
			Vector2 offset = EvaluationHelper.AsVector2(character, Position, new Vector2(0, 0));

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character target = character.FilterEntityAsTarget(entity, target_id);
				if (target == null) continue;

				switch (BindPosition)
				{
					case BindToTargetPostion.Mid:
						offset += target.BasePlayer.Constants.Midposition;
						break;

					case BindToTargetPostion.Head:
						offset += target.BasePlayer.Constants.Headposition;
						break;

					case BindToTargetPostion.None:
					case BindToTargetPostion.Foot:
					default:
						break;
				}

				character.Bind.Set(target, offset, time, 0, false);
				break;
			}
		}

		public Evaluation.Expression Time
		{
			get { return m_time; }
		}

		public Evaluation.Expression TargetId
		{
			get { return m_targetid; }
		}

		public Evaluation.Expression Position
		{
			get { return m_position; }
		}

		public BindToTargetPostion BindPosition
		{
			get { return m_bindpos; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_targetid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly BindToTargetPostion m_bindpos;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_position;

		#endregion
	}
}