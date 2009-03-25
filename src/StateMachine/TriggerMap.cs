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
	[DebuggerTypeProxy(typeof(TriggerMap.DebuggerProxy))]
	class TriggerMap
	{
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

		public TriggerMap(SortedDictionary<Int32, List<Evaluation.Expression>> triggers)
		{
			if (triggers == null) throw new ArgumentNullException("triggers");

			m_triggers = new List<Trigger>(triggers.Keys.Count);
			foreach (var kvp in triggers) m_triggers.Add(new Trigger(kvp.Key, kvp.Value));

			m_isvalid = ValidCheck();
		}

		Boolean ValidCheck()
		{
			if (m_triggers.Count == 0) return false;

			Int32 indexnumber = 0;
			foreach (Trigger trigger in m_triggers)
			{
				if (trigger.Index == indexnumber)
				{
					indexnumber += 1;
				}
				else if (indexnumber == 0 && trigger.Index == 1)
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

			foreach (Trigger trigger in m_triggers)
			{
				if (trigger.Check(character) == true)
				{
					if (trigger.Index != 0) return true;
				}
				else
				{
					if (trigger.Index == 0) return false;
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
		readonly List<Trigger> m_triggers;

		#endregion
	}
}