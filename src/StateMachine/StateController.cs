using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.IO;

namespace xnaMugen.StateMachine
{
	internal abstract class StateController
	{
		protected StateController(StateSystem statesystem, string label, TextSection textsection)
		{
			if (statesystem == null) throw new ArgumentNullException(nameof(statesystem));
			if (label == null) throw new ArgumentNullException(nameof(label));
			if (textsection == null) throw new ArgumentNullException(nameof(textsection));

			m_statesystem = statesystem;
			m_textsection = textsection;
            m_persistence = textsection.GetAttribute("persistent", 1);
			m_ignorehitpause = textsection.GetAttribute("ignorehitpause", false);
			m_triggers = BuildTriggers(textsection);
			m_label = label;
		}

		public virtual bool IsValid()
		{
			return Triggers.IsValid;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ Label.GetHashCode();
		}

		public abstract void Run(Combat.Character character);

		private TriggerMap BuildTriggers(TextSection textsection)
		{
			if (textsection == null) throw new ArgumentNullException(nameof(textsection));

			var triggers = new SortedDictionary<int, List<Evaluation.Expression>>();

			var evalsystem = StateSystem.GetSubSystem<Evaluation.EvaluationSystem>();

			foreach (var parsedline in textsection.ParsedLines)
			{
				if (string.Compare(parsedline.Key, 0, "trigger", 0, 7, StringComparison.OrdinalIgnoreCase) != 0) continue;

				int triggernumber;
				if (string.Compare(parsedline.Key, 7, "all", 0, 3, StringComparison.OrdinalIgnoreCase) == 0) triggernumber = 0;
				else if (int.TryParse(parsedline.Key.Substring(7), out triggernumber) == false) continue;

				var trigger = evalsystem.CreateExpression(parsedline.Value);

				if (triggers.ContainsKey(triggernumber) == false) triggers.Add(triggernumber, new List<Evaluation.Expression>());
				triggers[triggernumber].Add(trigger);
			}

			return new TriggerMap(triggers);
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", GetType().Name, Label);
		}

		public StateSystem StateSystem => m_statesystem;

		public TextSection Text => m_textsection;

		public int Persistence => m_persistence;

		public bool IgnoreHitPause => m_ignorehitpause;

		public TriggerMap Triggers => m_triggers;

		public string Label => m_label;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StateSystem m_statesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly TextSection m_textsection;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_persistence;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_ignorehitpause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly TriggerMap m_triggers;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_label;

		#endregion
	}
}