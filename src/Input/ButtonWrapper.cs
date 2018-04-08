namespace xnaMugen.Input
{
	/// <summary>
	/// Encapsulates a reference to a ButtonMap and button on that map.
	/// </summary>
	internal struct ButtonWrapper
	{
		/// <summary>
		/// Initializes a new instance of this class for a player ButtonMap.
		/// </summary>
		/// <param name="buttonmap">The index of the ButtonMap.</param>
		/// <param name="button">The PlayerButton to reference.</param>
		public ButtonWrapper(int buttonmap, PlayerButton button)
		{
			MapIndex = buttonmap;
			ButtonIndex = (int)button;
		}

		/// <summary>
		/// Initializes a new instance of this class for a system ButtonMap.
		/// </summary>
		/// <param name="buttonmap">The index of the ButtonMap.</param>
		/// <param name="button">The SystemButton to reference.</param>
		public ButtonWrapper(int buttonmap, SystemButton button)
		{
			MapIndex = buttonmap;
			ButtonIndex = (int)button;
		}

		/// <summary>
		/// Returns the index of the ButtonMap.
		/// </summary>
		/// <returns>The index of the ButtonMap.</returns>
		public int MapIndex { get; }

		/// <summary>
		/// Returns the index of the button.
		/// </summary>
		/// <returns>The button index.</returns>
		public int ButtonIndex { get; }

		#region Fields

		#endregion
	}
}