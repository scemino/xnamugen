using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("BGPalFx")]
	internal class BGPalFx : StateController
	{
		public BGPalFx(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_paladd = textsection.GetAttribute<Evaluation.Expression>("add", null);
			m_palmul = textsection.GetAttribute<Evaluation.Expression>("mul", null);
			m_sineadd = textsection.GetAttribute<Evaluation.Expression>("sinadd", null);
			m_palinvert = textsection.GetAttribute<Evaluation.Expression>("invertall", null);
			m_palcolor = textsection.GetAttribute<Evaluation.Expression>("color", null);
		}

		public override void Run(Combat.Character character)
		{
			var time = EvaluationHelper.AsInt32(character, Time, -2);
			var paladd = EvaluationHelper.AsVector3(character, PaletteColorAdd, new Vector3(0, 0, 0));
			var palmul = EvaluationHelper.AsVector3(character, PaletteColorMultiply, new Vector3(255, 255, 255));
			var sinadd = EvaluationHelper.AsVector4(character, PaletteColorSineAdd, new Vector4(0, 0, 0, 1));
			var invert = EvaluationHelper.AsBoolean(character, PaletteColorInversion, false);
			var basecolor = EvaluationHelper.AsInt32(character, PaletteColor, 255);

			if (time < -1) return;

			var palfx = character.Engine.Stage.PaletteFx;
			palfx.Set(time, paladd, palmul, sinadd, invert, basecolor / 255.0f);
		}

		public Evaluation.Expression Time => m_time;

		public Evaluation.Expression PaletteColorAdd => m_paladd;

		public Evaluation.Expression PaletteColorMultiply => m_palmul;

		public Evaluation.Expression PaletteColorSineAdd => m_sineadd;

		public Evaluation.Expression PaletteColorInversion => m_palinvert;

		public Evaluation.Expression PaletteColor => m_palcolor;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_paladd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_palmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_sineadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_palinvert;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_palcolor;

		#endregion
	}
}