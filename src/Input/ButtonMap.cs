using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Input
{
	/// <summary>
	/// Encapsulates a collection of input callbacks.
	/// </summary>
	[DebuggerDisplay("Count = {m_buttonmap.Count}")]
	internal class ButtonMap
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		public ButtonMap()
		{
			m_buttonmap = new Dictionary<int, Action<bool>>();
		}

		/// <summary>
		/// Copies the input callback in the given buttonmap.
		/// </summary>
		/// <param name="map">The ButtonMap to copy from.</param>
		public void Set(ButtonMap map)
		{
			if (map == null) throw new ArgumentNullException(nameof(map));

			Clear();
			foreach (var iter in map.m_buttonmap) m_buttonmap.Add(iter.Key, iter.Value);
		}

		/// <summary>
		/// Removes all currently set input callbacks.
		/// </summary>
		public void Clear()
		{
			m_buttonmap.Clear();
		}

		/// <summary>
		/// Add a new input callback for a SystemButton.
		/// </summary>
		/// <param name="button">The button associated with the callback.</param>
		/// <param name="callback">The callback to be fired.</param>
		public void Add(SystemButton button, Action<bool> callback)
		{
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			Add((int)button, callback);
		}

		/// <summary>
		/// Add a new input callback for a PlayerButton.
		/// </summary>
		/// <param name="button">The button associated with the callback.</param>
		/// <param name="callback">The callback to be fired.</param>
		public void Add(PlayerButton button, Action<bool> callback)
		{
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			Add((int)button, callback);
		}

		private void Add(int index, Action<bool> callback)
		{
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			if (m_buttonmap.ContainsKey(index))
			{
				m_buttonmap[index] += callback;
			}
			else
			{
				m_buttonmap[index] = callback;
			}
		}

		/// <summary>
		/// Fires an input callback for a button if it exists.
		/// </summary>
		/// <param name="button">The button index associates with the callback.</param>
		/// <param name="pressed">Whether the key has pressed or released.</param>
		public void Call(int button, bool pressed)
		{
			Action<bool> callback;
			if (m_buttonmap.TryGetValue(button, out callback)) callback(pressed);
		}

		public Action<bool> GetCallback(int button)
		{
			Action<bool> callback;
			if (m_buttonmap.TryGetValue(button, out callback)) return callback;

			return null;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		private readonly Dictionary<int, Action<bool>> m_buttonmap;

		#endregion
	}
}