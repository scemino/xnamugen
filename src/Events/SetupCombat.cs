using System;
using System.Diagnostics;

namespace xnaMugen.Events
{
    internal class SetupCombat : Base
	{
		public SetupCombat(Combat.EngineInitialization initialization)
		{
			if (initialization == null) throw new ArgumentNullException(nameof(initialization));

			m_initialization = initialization;
		}

		public Combat.EngineInitialization Initialization => m_initialization;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Combat.EngineInitialization m_initialization;

		#endregion
	}
}