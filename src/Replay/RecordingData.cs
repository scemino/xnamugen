namespace xnaMugen.Replay
{
	internal class RecordingData
	{
		public RecordingData(int system, int player1, int player2, int player3, int player4)
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