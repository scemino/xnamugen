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
	abstract class StateController
	{
		protected StateController(StateSystem statesystem, String label, TextSection textsection)
		{
			if (statesystem == null) throw new ArgumentNullException("statesystem");
			if (label == null) throw new ArgumentNullException("label");
			if (textsection == null) throw new ArgumentNullException("textsection");

			m_statesystem = statesystem;
			m_textsection = textsection;
            m_persistence = textsection.GetAttribute<Int32>("persistent", 1);
			m_ignorehitpause = textsection.GetAttribute<Boolean>("ignorehitpause", false);
			m_triggers = BuildTriggers(textsection);
			m_label = label;
		}

		public virtual Boolean IsValid()
		{
			return Triggers.IsValid;
		}

		public override Int32 GetHashCode()
		{
			return GetType().GetHashCode() ^ Label.GetHashCode();
		}

		public abstract void Run(Combat.Character character);

		TriggerMap BuildTriggers(TextSection textsection)
		{
			if (textsection == null) throw new ArgumentNullException("textsection");

			StringComparer sc = StringComparer.OrdinalIgnoreCase;
			var triggers = new SortedDictionary<Int32, List<Evaluation.Expression>>();

			var evalsystem = StateSystem.GetSubSystem<Evaluation.EvaluationSystem>();

			foreach (KeyValuePair<String, String> parsedline in textsection.ParsedLines)
			{
				if (String.Compare(parsedline.Key, 0, "trigger", 0, 7, StringComparison.OrdinalIgnoreCase) != 0) continue;

				Int32 triggernumber = 0;
				if (String.Compare(parsedline.Key, 7, "all", 0, 3, StringComparison.OrdinalIgnoreCase) == 0) triggernumber = 0;
				else if (Int32.TryParse(parsedline.Key.Substring(7), out triggernumber) == false) continue;

				Evaluation.Expression trigger = evalsystem.CreateExpression(parsedline.Value);

				if (triggers.ContainsKey(triggernumber) == false) triggers.Add(triggernumber, new List<Evaluation.Expression>());
				triggers[triggernumber].Add(trigger);
			}

			return new TriggerMap(triggers);
		}

		public override String ToString()
		{
			return String.Format("{0} - {1}", GetType().Name, Label);
		}

		public StateSystem StateSystem
		{
			get { return m_statesystem; }
		}

		public IO.TextSection Text
		{
			get { return m_textsection; }
		}

        public Int32 Persistence
		{
			get { return m_persistence; }
		}

		public Boolean IgnoreHitPause
		{
			get { return m_ignorehitpause; }
		}

		public TriggerMap Triggers
		{
			get { return m_triggers; }
		}

		public String Label
		{
			get { return m_label; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StateSystem m_statesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly IO.TextSection m_textsection;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_persistence;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_ignorehitpause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TriggerMap m_triggers;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_label;

		#endregion
	}
}