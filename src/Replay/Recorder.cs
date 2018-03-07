using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace xnaMugen.Replay
{
	internal class Recorder
	{
		public Recorder(Menus.CombatScreen screen)
		{
			if (screen == null) throw new ArgumentNullException(nameof(screen));

			m_screen = screen;
			m_data = new LinkedList<RecordingData>();
			m_input = new int[5];
			m_isrecording = false;
		}

		public void Reset()
		{
			m_isrecording = false;
			m_initsettings = m_screen.FightEngine.Initialization;
			m_data.Clear();
			m_input.Initialize();
		}

		public void SetInput(Input.InputState inputstate)
		{
			if (inputstate == null) throw new ArgumentNullException(nameof(inputstate));

			foreach (PlayerButton button in Enum.GetValues(typeof(PlayerButton)))
			{
				var buttonindex = button;

				inputstate[1].Add(buttonindex, x => RecieveInput((int)buttonindex, 1, x));
				inputstate[2].Add(buttonindex, x => RecieveInput((int)buttonindex, 2, x));
				inputstate[3].Add(buttonindex, x => RecieveInput((int)buttonindex, 3, x));
				inputstate[4].Add(buttonindex, x => RecieveInput((int)buttonindex, 4, x));
			}

			foreach (SystemButton button in Enum.GetValues(typeof(SystemButton)))
			{
				var buttonindex = button;

				inputstate[0].Add(buttonindex, x => RecieveInput((int)buttonindex, 0, x));
			}
		}

		public void Update()
		{
			var data = new RecordingData(m_input[0], m_input[1], m_input[2], m_input[3], m_input[4]);
			m_data.AddLast(data);
		}

		public void StartRecording()
		{
			IsRecording = true;
		}

		public void EndRecording()
		{
			if (IsRecording == false) return;

			IsRecording = false;

			var filename = string.Format("xnaMugen Replay - {0:u}.txt", DateTime.Now).Replace(':', '-');

			using (var writer = new StreamWriter(filename))
			{
				writer.WriteLine("[xnaMugen Replay Header]");
				writer.WriteLine("Version = 1");
				writer.WriteLine("Combat Mode = {0}", m_initsettings.Mode);
				writer.WriteLine("Player 1 Name = {0}", m_initsettings.Team1P1.Profile.PlayerName);
                writer.WriteLine("Player 1 Version = {0}", m_initsettings.Team1P1.Profile.Version);
                writer.WriteLine("Player 1 Palette = {0}", m_initsettings.Team1P1.PaletteIndex);
                writer.WriteLine("Player 2 Name = {0}", m_initsettings.Team2P1.Profile.PlayerName);
                writer.WriteLine("Player 2 Version = {0}", m_initsettings.Team2P1.Profile.Version);
                writer.WriteLine("Player 2 Palette = {0}", m_initsettings.Team2P1.PaletteIndex);
				writer.WriteLine("Stage Path = {0}", m_initsettings.Stage.Filepath);
				writer.WriteLine("Seed = {0}", m_initsettings.Seed);

				writer.WriteLine();
				writer.WriteLine("[xnaMugen Replay Data]");

				foreach (var data in m_data)
				{
					writer.WriteLine("{0}, {1}, {2}, {3}, {4}", (int)data.SystemInput, (int)data.Player1Input, (int)data.Player2Input, (int)data.Player3Input, (int)data.Player4Input);
				}
			}

			m_data.Clear();
			m_input.Initialize();
		}

		private void RecieveInput(int buttonindex, int playerindex, bool pressed)
		{
			if (pressed)
			{
				m_input[playerindex] |= buttonindex;
			}
			else
			{
				m_input[playerindex] &= ~buttonindex;
			}
		}

		public bool IsRecording
		{
			get => m_isrecording;

			private set { m_isrecording = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Menus.CombatScreen m_screen;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly LinkedList<RecordingData> m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int[] m_input;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Combat.EngineInitialization m_initsettings;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_isrecording;

		#endregion
	}
}