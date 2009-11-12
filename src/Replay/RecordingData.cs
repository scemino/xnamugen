using System;

namespace xnaMugen.Replay
{
	class RecordingData
	{
		public RecordingData(Int32 system, Int32 player1, Int32 player2, Int32 player3, Int32 player4)
		{
			SystemInput = (SystemButton)system;
			Player1Input = (PlayerButton)player1;
			Player2Input = (PlayerButton)player2;
			Player3Input = (PlayerButton)player3;
			Player4Input = (PlayerButton)player4;
		}

		public readonly SystemButton SystemInput;

		public readonly PlayerButton Player1Input;

		public readonly PlayerButton Player2Input;

		public readonly PlayerButton Player3Input;

		public readonly PlayerButton Player4Input;
	}
}