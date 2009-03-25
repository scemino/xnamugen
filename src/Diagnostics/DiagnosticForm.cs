using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace xnaMugen.Diagnostics
{
	class DiagnosticForm : Form
	{
		public DiagnosticForm()
		{
			m_generalpanel = new GeneralPanel();

			this.Width = 400;
			this.Height = 500;

			this.Text = "xnaMugen Diagnostic Window";
			this.Controls.Add(m_generalpanel);

			m_generalpanel.Dock = DockStyle.Fill;
		}

		public void Set(Combat.FightEngine engine)
		{
			if (engine == null) throw new ArgumentNullException("engine");

			m_generalpanel.Set(engine);
		}

		#region Fields

		readonly GeneralPanel m_generalpanel;

		#endregion
	}
}