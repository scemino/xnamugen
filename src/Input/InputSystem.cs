using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using xnaMugen.Collections;

namespace xnaMugen.Input
{
	/// <summary>
	/// Interfaces between keyboard input and game code.
	/// </summary>
	class InputSystem : SubSystem
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="subsystems">The collection of subsystems.</param>
		public InputSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_keymap = new Dictionary<Keys, ButtonWrapper>();
			m_previousstate = new KeyboardState();
			m_inputstatestack = new Stack<InputState>();
			m_currentinput = new InputState();
			m_callbackcache = new List<Action<Boolean>>();
		}

		public override void Initialize()
		{
			InitializationSettings settings = GetSubSystem<InitializationSettings>();

			m_keymap.Clear();

			foreach (var kvp in settings.SystemKeys)
			{
				m_keymap.Add(kvp.Value, new ButtonWrapper(0, kvp.Key));
			}

			foreach (var kvp in settings.Player1Keys)
			{
				m_keymap.Add(kvp.Value, new ButtonWrapper(1, kvp.Key));
			}

			foreach (var kvp in settings.Player2Keys)
			{
				m_keymap.Add(kvp.Value, new ButtonWrapper(2, kvp.Key));
			}

#if FRANTZX
			m_keymap.Add(Keys.P, new ButtonWrapper(2, PlayerButton.Y));
			m_keymap.Add(Keys.OemOpenBrackets, new ButtonWrapper(0, SystemButton.Pause));
#endif
		}

		/// <summary>
		/// Saves current input state.
		/// </summary>
		public void SaveInputState()
		{
			m_inputstatestack.Push(m_currentinput.Clone());
		}

		/// <summary>
		/// Restores last saved input state.
		/// </summary>
		public void LoadInputState()
		{
			if (m_inputstatestack.Count == 0) throw new InvalidOperationException();

			m_currentinput.Set(m_inputstatestack.Pop());
		}

		/// <summary>
		/// Checks for keyboard input and fires events based on that input.
		/// </summary>
		public void Update()
		{
			KeyboardState ks = Keyboard.GetState();
			m_callbackcache.Clear();

			foreach (KeyValuePair<Keys, ButtonWrapper> mapvalue in m_keymap)
			{
				if (ks[mapvalue.Key] == m_previousstate[mapvalue.Key]) continue;

				ButtonMap buttonmap = CurrentInput[mapvalue.Value.MapIndex];

				Boolean pressed = ks[mapvalue.Key] == KeyState.Down;

				Action<Boolean> callback = buttonmap.GetCallback(mapvalue.Value.ButtonIndex);

				if (callback != null && m_callbackcache.Contains(callback) == false)
				{
					callback(pressed);
					m_callbackcache.Add(callback);
				}
			}

			m_previousstate = ks;
		}

		/// <summary>
		/// Returns the current input state.
		/// </summary>
		/// <returns>The current input state.</returns>
		public InputState CurrentInput
		{
			get { return m_currentinput; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<Keys, ButtonWrapper> m_keymap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		KeyboardState m_previousstate;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly InputState m_currentinput;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Stack<InputState> m_inputstatestack;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Action<Boolean>> m_callbackcache;

		#endregion
	}
}