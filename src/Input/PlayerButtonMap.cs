using System;
using System.Collections.Generic;

namespace xnaMugen.Input
{
	internal class PlayerButtonMap
	{
		public PlayerButtonMap()
		{
			m_callbackmap = new Dictionary<int, Action<bool>>();
		}

		public void Clear()
		{
			m_callbackmap.Clear();

			m_callback = null;
		}

		public void Add(PlayerButton button, Action<bool> callback)
		{
			if (button == PlayerButton.None) throw new ArgumentOutOfRangeException(nameof(button));
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			m_callbackmap[(int)button] = callback;
		}

		public void Add(PlayerButtonDelegate callback)
		{
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			m_callback = callback;
		}

		public void Call(PlayerButton button, bool pressed)
		{
			if (button == PlayerButton.None) throw new ArgumentOutOfRangeException(nameof(button));

			if (m_callback != null)
			{
				m_callback(button, pressed);
			}
			else
			{
				Action<bool> callback;
				if (m_callbackmap.TryGetValue((int)button, out callback)) callback(pressed);
			}
		}

		#region Fields

		private PlayerButtonDelegate m_callback;

		private readonly Dictionary<int, Action<bool>> m_callbackmap;

		#endregion
	}
}