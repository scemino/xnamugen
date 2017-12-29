using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Text.RegularExpressions;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("VarAdd")]
	internal class VarAdd : StateController
	{
		static VarAdd()
		{
			s_lineregex = new Regex(@"(.*)?var\((.+)\)", RegexOptions.IgnoreCase);
		}

		public VarAdd(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_intnumber = textsection.GetAttribute<Evaluation.Expression>("v", null);
			m_floatnumber = textsection.GetAttribute<Evaluation.Expression>("fv", null);
			m_systemintnumber = textsection.GetAttribute<Evaluation.Expression>("sysvar", null);
			m_systemfloatnumber = textsection.GetAttribute<Evaluation.Expression>("sysfvar", null);
			m_value = textsection.GetAttribute<Evaluation.Expression>("value", null);

			foreach (var parsedline in textsection.ParsedLines)
			{
				var match = s_lineregex.Match(parsedline.Key);
				if (match.Success == false) continue;

				var evalsystem = StateSystem.GetSubSystem<Evaluation.EvaluationSystem>();
				var sc = StringComparer.OrdinalIgnoreCase;
				var varType = match.Groups[1].Value;
				var varNumber = evalsystem.CreateExpression(match.Groups[2].Value);

				if (sc.Equals(varType, "")) m_intnumber = varNumber;
				if (sc.Equals(varType, "f")) m_floatnumber = varNumber;
				if (sc.Equals(varType, "sys")) m_systemintnumber = varNumber;
				if (sc.Equals(varType, "sysf")) m_systemfloatnumber = varNumber;

				m_value = evalsystem.CreateExpression(parsedline.Value);
			}
		}

		public override void Run(Combat.Character character)
		{
			if (IntNumber != null)
			{
				var index = EvaluationHelper.AsInt32(character, IntNumber, null);
				var value = EvaluationHelper.AsInt32(character, Value, null);

				if (index != null && value != null &&character.Variables.AddInteger(index.Value, false, value.Value) == false)
				{
				}
			}

			if (FloatNumber != null)
			{
				var index = EvaluationHelper.AsInt32(character, FloatNumber, null);
				var value = EvaluationHelper.AsSingle(character, Value, null);

				if (index != null && value != null && character.Variables.AddFloat(index.Value, false, value.Value) == false)
				{
				}
			}

			if (SystemIntNumber != null)
			{
				var index = EvaluationHelper.AsInt32(character, SystemIntNumber, null);
				var value = EvaluationHelper.AsInt32(character, Value, null);

				if (index != null && value != null && character.Variables.AddInteger(index.Value, true, value.Value) == false)
				{
				}
			}

			if (SystemFloatNumber != null)
			{
				var index = EvaluationHelper.AsInt32(character, SystemFloatNumber, null);
				var value = EvaluationHelper.AsSingle(character, Value, null);

				if (index != null && value != null && character.Variables.AddFloat(index.Value, true, value.Value) == false)
				{
				}
			}
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Value == null) return false;

			var count = 0;
			if (IntNumber != null) ++count;
			if (FloatNumber != null) ++count;
			if (SystemIntNumber != null) ++count;
			if (SystemFloatNumber != null) ++count;
			if (count != 1) return false;	

			return true;
		}

		public Evaluation.Expression IntNumber => m_intnumber;

		public Evaluation.Expression FloatNumber => m_floatnumber;

		public Evaluation.Expression SystemIntNumber => m_systemintnumber;

		public Evaluation.Expression SystemFloatNumber => m_systemfloatnumber;

		public Evaluation.Expression Value => m_value;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Regex s_lineregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_intnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_floatnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_systemintnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_systemfloatnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_value;

		#endregion
	}
}