using System;
using Microsoft.Xna.Framework;
using Mono.Options;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Assembly")]

namespace xnaMugen
{
	/// <summary>
	/// Class holding entry point into program.
	/// </summary>
	internal static class EntryPoint
	{
		/// <summary>
		/// Where the program starts. Starts logging if necessary and runs game loop.
		/// </summary>
		private static void Main(string[] args)
		{
			var mugenOptions = new MugenOptions();
			var shouldShowHelp = false;
			var options = new OptionSet
			{ 
				{ "h|help", "Displays help", h => shouldShowHelp = h != null },
				{ "p3|player3=", "Loads a character named VALUE as player3", name => mugenOptions.Player3 = name }, 
				{ "p4|player4=", "Loads a character named VALUE as player4", name => mugenOptions.Player4 = name }, 
				{ "s|stage=", "Loads a stage named VALUE.def in stages/", s => mugenOptions.Stage = s }, 
			};

			try
			{
				// parse the command line
				var extra = options.Parse(args);
				if (extra.Count > 1)
				{
					mugenOptions.Player1 = extra[0];
					mugenOptions.Player2 = extra[1];
				}
				else if (extra.Count > 0)
				{
					mugenOptions.Player1 = extra[0];
				}
			}
			catch (OptionException e)
			{
				// output some error message
				Console.Write("xnaMugen: ");
				Console.WriteLine(e.Message);
				Console.WriteLine("Try `xnaMugen --help' for more information.");
				return;
			}

			if (shouldShowHelp)
			{
				ShowHelp(options);
				return;
			}
#if DEBUG
			Log.Start();
			using (Game g = new Mugen(mugenOptions)) g.Run();
#else
			try
			{
				Log.Start();
				using (Game g = new Mugen(mugenOptions)) g.Run();
			}
			catch (Exception e)
			{
				Log.WriteException(e);

				System.Windows.Forms.MessageBox.Show(e.ToString(), "xnaMugen");
			}
#endif
		}

		private static void ShowHelp(OptionSet options)
		{
			// show some app description message
			Console.WriteLine("Usage: xnaMugen [player1 player2 [Options]]");
			Console.WriteLine("For example, to start quick versus between players named KFM and");
			Console.WriteLine("Suave, you can type:");
			Console.WriteLine();
			Console.WriteLine("./xnaMugen kfm suave");
			Console.WriteLine();
			Console.WriteLine("To play the same two characters in a stage named TEMPLE, type:");
			Console.WriteLine();
			Console.WriteLine("./xnaMugen kfm suave -s temple");
			Console.WriteLine();

			// output the options
			Console.WriteLine("Options:");
			options.WriteOptionDescriptions(Console.Out);
		}
	}
}