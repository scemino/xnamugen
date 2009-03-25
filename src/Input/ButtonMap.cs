using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Input
{
	/// <summary>
	/// Encapsulates a collection of input callbacks.
	/// </summary>
	[DebuggerDisplay("Count = {m_buttonmap.Count}")]
	class ButtonMap
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		public ButtonMap()
		{
			m_buttonmap = new Dictionary<Int32, Action<Boolean>>();
		}

		/// <summary>
		/// Copies the input callback in the given buttonmap.
		/// </summary>
		/// <param name="map">The ButtonMap to copy from.</param>
		public void Set(ButtonMap map)
		{
			if (map == null) throw new ArgumentNullException("map");

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
		public void Add(SystemButton button, Action<Boolean> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");

			m_buttonmap[(Int32)button] = callback;
		}

		/// <summary>
		/// Add a new input callback for a PlayerButton.
		/// </summary>
		/// <param name="button">The button associated with the callback.</param>
		/// <param name="callback">The callback to be fired.</param>
		public void Add(PlayerButton button, Action<Boolean> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");

			m_buttonmap[(Int32)button] = callback;
		}

		/// <summary>
		/// Fires an input callback for a button if it exists.
		/// </summary>
		/// <param name="button">The button index associates with the callback.</param>
		/// <param name="pressed">Whether the key has pressed or released.</param>
		public void Call(Int32 button, Boolean pressed)
		{
			Action<Boolean> callback;
			if (m_buttonmap.TryGetValue(button, out callback) == true) callback(pressed);
		}

		public Action<Boolean> GetCallback(Int32 button)
		{
			Action<Boolean> callback;
			if (m_buttonmap.TryGetValue(button, out callback) == true) return callback;

			return null;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly Dictionary<Int32, Action<Boolean>> m_buttonmap;

		#endregion
	}
}