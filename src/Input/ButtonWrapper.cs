using System;

namespace xnaMugen.Input
{
	/// <summary>
	/// Encapsulates a reference to a ButtonMap and button on that map.
	/// </summary>
	struct ButtonWrapper
	{
		/// <summary>
		/// Initializes a new instance of this class for a player ButtonMap.
		/// </summary>
		/// <param name="buttonmap">The index of the ButtonMap.</param>
		/// <param name="button">The PlayerButton to reference.</param>
		public ButtonWrapper(Int32 buttonmap, PlayerButton button)
		{
			m_buttonmap = buttonmap;
			m_button = (Int32)button;
		}

		/// <summary>
		/// Initializes a new instance of this class for a system ButtonMap.
		/// </summary>
		/// <param name="buttonmap">The index of the ButtonMap.</param>
		/// <param name="button">The SystemButton to reference.</param>
		public ButtonWrapper(Int32 buttonmap, SystemButton button)
		{
			m_buttonmap = buttonmap;
			m_button = (Int32)button;
		}

		public ButtonWrapper(Int32 buttonmap, Int32 buttonindex)
		{
			m_buttonmap = buttonmap;
			m_button = buttonindex;
		}

		/// <summary>
		/// Returns the index of the ButtonMap.
		/// </summary>
		/// <returns>The index of the ButtonMap.</returns>
		public Int32 MapIndex
		{
			get { return m_buttonmap; }
		}

		/// <summary>
		/// Returns the index of the button.
		/// </summary>
		/// <returns>The button index.</returns>
		public Int32 ButtonIndex
		{
			get { return m_button; }
		}

		#region Fields

		readonly Int32 m_buttonmap;

		readonly Int32 m_button;

		#endregion
	}
}