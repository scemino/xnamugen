using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.StateMachine
{
	//[DebuggerTypeProxy(typeof(TriggerMap.DebuggerProxy))]
	internal class TriggerMap
	{
		/*
		class DebuggerProxy
		{
			public DebuggerProxy(TriggerMap triggermap)
			{
				if (triggermap == null) throw new ArgumentNullException("triggermap");

				m_triggers = triggermap.m_triggers.ToArray();
			}

			#region Fields

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			readonly Trigger[] m_triggers;

			#endregion
		}
		*/

		public TriggerMap(SortedDictionary<int, List<Evaluation.Expression>> triggers)
		{
			if (triggers == null) throw new ArgumentNullException(nameof(triggers));

			m_triggers = triggers;
			m_isvalid = ValidCheck();
		}

		private bool ValidCheck()
		{
			if (m_triggers.Count == 0) return false;

			var indexnumber = 0;
			foreach (var trigger in m_triggers)
			{
				if (trigger.Key == indexnumber)
				{
					indexnumber += 1;
				}
				else if (indexnumber == 0 && trigger.Key == 1)
				{
					indexnumber = 2;
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		public bool Trigger(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			foreach (var trigger in m_triggers)
			{
				var ok = true;
				foreach (var exp in trigger.Value)
				{
					if(exp.EvaluateFirst(character).BooleanValue == false)
					{
						ok = false;
						break;
					}
				}

				if (ok)
				{
					if (trigger.Key != 0) return true;
				}
				else
				{
					if (trigger.Key == 0) return false;
				}
			}

			return false;
		}

		public bool IsValid => m_isvalid;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_isvalid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SortedDictionary<int, List<Evaluation.Expression>> m_triggers;

		#endregion
	}
}