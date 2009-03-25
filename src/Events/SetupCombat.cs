using System;
using System.Diagnostics;

namespace xnaMugen.Events
{
	class SetupCombat : Base
	{
		public SetupCombat(Combat.EngineInitialization initialization)
		{
			if (initialization == null) throw new ArgumentNullException("initialization");

			m_initialization = initialization;
		}

		public Combat.EngineInitialization Initialization
		{
			get { return m_initialization; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Combat.EngineInitialization m_initialization;

		#endregion
	}
}