using System;
using System.Collections.Generic;
using xnaMugen.Collections;
using System.Diagnostics;

namespace xnaMugen.Input
{
	/// <summary>
	/// Encapsulates a collection of ButtonMaps.
	/// </summary>
	class InputState
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		public InputState()
		{
			m_buttonmaps = new List<ButtonMap>(5) { new ButtonMap(), new ButtonMap(), new ButtonMap(), new ButtonMap(), new ButtonMap() };
		}

		/// <summary>
		/// Creates a new InputState whose ButtonMaps are a copy of this instance's.
		/// </summary>
		/// <returns>A copy of this instance.</returns>
		public InputState Clone()
		{
			InputState state = new InputState();

			state.Set(this);

			return state;
		}

		/// <summary>
		/// Copies the input callbacks of the given InputState.
		/// </summary>
		/// <param name="state">The InputState to copy.</param>
		public void Set(InputState state)
		{
			if (state == null) throw new ArgumentNullException("state");

			for (Int32 i = 0; i != m_buttonmaps.Count; ++i)
			{
				this[i].Set(state[i]);
			}
		}

		/// <summary>
		/// Returns an enumerator for iteration through the contained ButtonMaps.
		/// </summary>
		/// <returns></returns>
		public List<ButtonMap>.Enumerator GetEnumerator()
		{
			return m_buttonmaps.GetEnumerator();
		}

		/// <summary>
		/// Returns the ButtonMap with a given index.
		/// </summary>
		/// <param name="index">The index of the requested ButtonMap.</param>
		/// <returns>The requested ButtonMap.</returns>
		public ButtonMap this[Int32 index]
		{
			get { return m_buttonmaps[index]; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly List<ButtonMap> m_buttonmaps;

		#endregion
	}
}