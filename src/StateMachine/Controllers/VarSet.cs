using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("VarSet")]
	class VarSet : StateController
	{
		static VarSet()
		{
			s_lineregex = new Regex(@"(.*)?var\((.+)\)", RegexOptions.IgnoreCase);
		}

		public VarSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_intnumber = textsection.GetAttribute<Evaluation.Expression>("v", null);
			m_floatnumber = textsection.GetAttribute<Evaluation.Expression>("fv", null);
			m_systemintnumber = textsection.GetAttribute<Evaluation.Expression>("sysvar", null);
			m_systemfloatnumber = textsection.GetAttribute<Evaluation.Expression>("sysfvar", null);
			m_value = textsection.GetAttribute<Evaluation.Expression>("value", null);

			foreach(KeyValuePair<String, String> parsedline in textsection.ParsedLines)
			{
				Match match = s_lineregex.Match(parsedline.Key);
				if (match.Success == false) continue;

				Evaluation.EvaluationSystem evalsystem = StateSystem.GetSubSystem<Evaluation.EvaluationSystem>();
				StringComparer sc = StringComparer.OrdinalIgnoreCase;
				String var_type = match.Groups[1].Value;
				Evaluation.Expression var_number = evalsystem.CreateExpression(match.Groups[2].Value);

				if (sc.Equals(var_type, "") == true) m_intnumber = var_number;
				if (sc.Equals(var_type, "f") == true) m_floatnumber = var_number;
				if (sc.Equals(var_type, "sys") == true) m_systemintnumber = var_number;
				if (sc.Equals(var_type, "sysf") == true) m_systemfloatnumber = var_number;

				m_value = evalsystem.CreateExpression(parsedline.Value);
			}
		}

		public override void Run(Combat.Character character)
		{
			if (IntNumber != null)
			{
				Int32? index = EvaluationHelper.AsInt32(character, IntNumber, null);
				Int32? value = EvaluationHelper.AsInt32(character, Value, null);

				if (index != null && value != null && character.Variables.SetInteger(index.Value, false, value.Value) == false)
				{
				}
			}

			if (FloatNumber != null)
			{
				Int32? index = EvaluationHelper.AsInt32(character, FloatNumber, null);
				Single? value = EvaluationHelper.AsSingle(character, Value, null);

				if (index != null && value != null && character.Variables.SetFloat(index.Value, false, value.Value) == false)
				{
				}
			}

			if (SystemIntNumber != null)
			{
				Int32? index = EvaluationHelper.AsInt32(character, SystemIntNumber, null);
				Int32? value = EvaluationHelper.AsInt32(character, Value, null);

				if (index != null && value != null && character.Variables.SetInteger(index.Value, true, value.Value) == false)
				{
				}
			}

			if (SystemFloatNumber != null)
			{
				Int32? index = EvaluationHelper.AsInt32(character, SystemFloatNumber, null);
				Single? value = EvaluationHelper.AsSingle(character, Value, null);

				if (index != null && value != null && character.Variables.SetFloat(index.Value, true, value.Value) == false)
				{
				}
			}
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Value == null) return false;

			Int32 count = 0;
			if (IntNumber != null) ++count;
			if (FloatNumber != null) ++count;
			if (SystemIntNumber != null) ++count;
			if (SystemFloatNumber != null) ++count;
			if (count != 1) return false;	

			return true;
		}

		public Evaluation.Expression IntNumber
		{
			get { return m_intnumber; }
		}

		public Evaluation.Expression FloatNumber
		{
			get { return m_floatnumber; }
		}

		public Evaluation.Expression SystemIntNumber
		{
			get { return m_systemintnumber; }
		}

		public Evaluation.Expression SystemFloatNumber
		{
			get { return m_systemfloatnumber; }
		}

		public Evaluation.Expression Value
		{
			get { return m_value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static Regex s_lineregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_intnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_floatnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_systemintnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_systemfloatnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_value;

		#endregion
	}
}