using System;
using System.Windows.Forms;

namespace xnaMugen.Diagnostics
{
	internal class DiagnosticForm : Form
	{
		public DiagnosticForm()
		{
			m_generalpanel = new GeneralPanel();

			Width = 400;
			Height = 500;

			Text = "xnaMugen Diagnostic Window";
			Controls.Add(m_generalpanel);

			m_generalpanel.Dock = DockStyle.Fill;
		}

		public void Set(Combat.FightEngine engine)
		{
			if (engine == null) throw new ArgumentNullException(nameof(engine));

			m_generalpanel.Set(engine);
		}

		#region Fields

		private readonly GeneralPanel m_generalpanel;

		#endregion
	}
}