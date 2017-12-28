using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ReversalDef")]
	class ReversalDef : StateController
	{
		public ReversalDef(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_pausetime = textsection.GetAttribute<Evaluation.Expression>("pausetime", null);
			m_sparknumber = textsection.GetAttribute<Evaluation.PrefixedExpression>("sparkno", null);
			m_hitsound = textsection.GetAttribute<Evaluation.PrefixedExpression>("hitsound", null);
			m_p1statenumber = textsection.GetAttribute<Evaluation.Expression>("p1stateno", null);
			m_p2statenumber = textsection.GetAttribute<Evaluation.Expression>("p2stateno", null);
			m_hitattr = textsection.GetAttribute<Combat.HitAttribute>("reversal.attr", null);
		}

		public override void Run(Combat.Character character)
		{
		}

		public Evaluation.Expression PauseTime
		{
			get { return m_pausetime; }
		}

		public Evaluation.PrefixedExpression SparkNumber
		{
			get { return m_sparknumber; }
		}

		public Evaluation.PrefixedExpression HitSound
		{
			get { return m_hitsound; }
		}

		public Evaluation.Expression P1StateNumber
		{
			get { return m_p1statenumber; }
		}

		public Evaluation.Expression P2StateNumber
		{
			get { return m_p2statenumber; }
		}

		public Combat.HitAttribute ReversalHitAttribute
		{
			get { return m_hitattr; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Evaluation.Expression m_pausetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.PrefixedExpression m_sparknumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.PrefixedExpression m_hitsound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p1statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_p2statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Combat.HitAttribute m_hitattr;

		#endregion
	}
}