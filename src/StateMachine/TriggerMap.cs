using System;
using System.Diagnostics;
using xnaMugen.Collections;
using System.Collections.Generic;
using xnaMugen.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine
{
	//[DebuggerTypeProxy(typeof(TriggerMap.DebuggerProxy))]
	class TriggerMap
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

		public TriggerMap(SortedDictionary<Int32, List<Evaluation.Expression>> triggers)
		{
			if (triggers == null) throw new ArgumentNullException("triggers");

			m_triggers = triggers;
			m_isvalid = ValidCheck();
		}

		Boolean ValidCheck()
		{
			if (m_triggers.Count == 0) return false;

			Int32 indexnumber = 0;
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

		public Boolean Trigger(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			foreach (var trigger in m_triggers)
			{
				Boolean ok = true;
				foreach (Evaluation.Expression exp in trigger.Value)
				{
					if(exp.EvaluateFirst(character).BooleanValue == false)
					{
						ok = false;
						break;
					}
				}

				if (ok == true)
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

		public Boolean IsValid
		{
			get { return m_isvalid; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_isvalid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SortedDictionary<Int32, List<Evaluation.Expression>> m_triggers;

		#endregion
	}
}