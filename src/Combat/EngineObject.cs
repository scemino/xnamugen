using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal abstract class EngineObject
	{
		protected EngineObject(FightEngine engine)
		{
			if (engine == null) throw new ArgumentNullException(nameof(engine));

			m_engine = engine;
		}

		public FightEngine Engine => m_engine;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly FightEngine m_engine;

		#endregion
	}
}