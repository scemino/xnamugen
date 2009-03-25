using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Input
{
	class SystemButtonMap
	{
		public SystemButtonMap()
		{
			m_callbackmap = new Dictionary<Int32, Action<Boolean>>();
		}

		public void Clear()
		{
			m_callbackmap.Clear();

			m_callback = null;
		}

		public void Add(SystemButton button, Action<Boolean> callback)
		{
			if (button == SystemButton.None) throw new ArgumentOutOfRangeException("button");
			if (callback == null) throw new ArgumentNullException("callback");

			m_callbackmap[(Int32)button] = callback;
		}

		public void Add(SystemButtonDelegate callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");

			m_callback = callback;
		}

		public void Call(SystemButton button, Boolean pressed)
		{
			if (button == SystemButton.None) throw new ArgumentOutOfRangeException("button");

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

		SystemButtonDelegate m_callback;

		readonly Dictionary<Int32, Action<Boolean>> m_callbackmap;

		#endregion
	}
}