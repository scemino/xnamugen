using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Combat
{
	abstract class EngineObject
	{
		protected EngineObject(FightEngine engine)
		{
			if (engine == null) throw new ArgumentNullException("fightengine");

			m_engine = engine;
		}

		public FightEngine Engine
		{
			get { return m_engine; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly FightEngine m_engine;

		#endregion
	}
}