using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace xnaMugen.Replay
{
	class Recorder
	{
		public Recorder(Menus.CombatScreen screen)
		{
			if (screen == null) throw new ArgumentNullException("screen");

			m_screen = screen;
			m_data = new LinkedList<RecordingData>();
			m_input = new Int32[5];
		}

		public void Reset()
		{
			m_initsettings = m_screen.FightEngine.Initialization;
			m_data.Clear();
			m_input.Initialize();
		}

		public void SetInput(Input.InputState inputstate)
		{
			if (inputstate == null) throw new ArgumentNullException("inputstate");

			foreach (PlayerButton button in Enum.GetValues(typeof(PlayerButton)))
			{
				PlayerButton buttonindex = button;

				inputstate[1].Add(buttonindex, x => RecieveInput((Int32)buttonindex, 1, x));
				inputstate[2].Add(buttonindex, x => RecieveInput((Int32)buttonindex, 2, x));
				inputstate[3].Add(buttonindex, x => RecieveInput((Int32)buttonindex, 3, x));
				inputstate[4].Add(buttonindex, x => RecieveInput((Int32)buttonindex, 4, x));
			}

			foreach (SystemButton button in Enum.GetValues(typeof(SystemButton)))
			{
				SystemButton buttonindex = button;

				inputstate[0].Add(buttonindex, x => RecieveInput((Int32)buttonindex, 0, x));
			}
		}

		public void Update()
		{
			RecordingData data = new RecordingData(m_input[0], m_input[1], m_input[2], m_input[3], m_input[4]);
			m_data.AddLast(data);
		}

		public void EndRecording()
		{
			String filename = String.Format("xnaMugen Replay - {0:u}.txt", DateTime.Now).Replace(':', '-');

			using (StreamWriter writer = new StreamWriter(filename))
			{
				writer.WriteLine("[xnaMugen Replay Header]");
				writer.WriteLine("Version = 1");
				writer.WriteLine("Combat Mode = {0}", m_initsettings.Mode);
				writer.WriteLine("Player 1 Name = {0}", m_initsettings.P1.Profile.PlayerName);
				writer.WriteLine("Player 1 Version = {0}", m_initsettings.P1.Profile.Version);
				writer.WriteLine("Player 1 Palette = {0}", m_initsettings.P1.PaletteIndex);
				writer.WriteLine("Player 2 Name = {0}", m_initsettings.P2.Profile.PlayerName);
				writer.WriteLine("Player 2 Version = {0}", m_initsettings.P2.Profile.Version);
				writer.WriteLine("Player 2 Palette = {0}", m_initsettings.P2.PaletteIndex);
				writer.WriteLine("Stage Path = {0}", m_initsettings.Stage.Filepath);
				writer.WriteLine("Seed = {0}", m_initsettings.Seed);

				writer.WriteLine();
				writer.WriteLine("[xnaMugen Replay Data]");

				foreach (RecordingData data in m_data)
				{
					writer.WriteLine("{0}, {1}, {2}, {3}, {4}", (Int32)data.SystemInput, (Int32)data.Player1Input, (Int32)data.Player2Input, (Int32)data.Player3Input, (Int32)data.Player4Input);
				}
			}

			m_data.Clear();
			m_input.Initialize();
		}

		void RecieveInput(Int32 buttonindex, Int32 playerindex, Boolean pressed)
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

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Menus.CombatScreen m_screen;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly LinkedList<RecordingData> m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32[] m_input;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Combat.EngineInitialization m_initsettings;

		#endregion
	}
}