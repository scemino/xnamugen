using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Input
{
	class PlayerButtonMap
	{
		public PlayerButtonMap()
		{
			m_callbackmap = new Dictionary<Int32, Action<Boolean>>();
		}

		public void Clear()
		{
			m_callbackmap.Clear();

			m_callback = null;
		}

		public void Add(PlayerButton button, Action<Boolean> callback)
		{
			if (button == PlayerButton.None) throw new ArgumentOutOfRangeException("button");
			if (callback == null) throw new ArgumentNullException("callback");

			m_callbackmap[(Int32)button] = callback;
		}

		public void Add(PlayerButtonDelegate callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");

			m_callback = callback;
		}

		public void Call(PlayerButton button, Boolean pressed)
		{
			if (button == PlayerButton.None) throw new ArgumentOutOfRangeException("button");

			if (m_callback != null)
			{
				m_callback(button, pressed);
			}
			else
			{
				Action<Boolean> callback;
				if (m_callbackmap.TryGetValue((Int32)button, out callback) == true) callback(pressed);
			}
		}

		#region Fields

		PlayerButtonDelegate m_callback;

		readonly Dictionary<Int32, Action<Boolean>> m_callbackmap;

		#endregion
	}
}