using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("BGPalFx")]
	class BGPalFx : StateController
	{
		public BGPalFx(StateSystem statesystem, String label, TextSection textsection)
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
			Int32 time = EvaluationHelper.AsInt32(character, Time, -2);
			Vector3 paladd = EvaluationHelper.AsVector3(character, PaletteColorAdd, new Vector3(0, 0, 0));
			Vector3 palmul = EvaluationHelper.AsVector3(character, PaletteColorMultiply, new Vector3(255, 255, 255));
			Vector4 sinadd = EvaluationHelper.AsVector4(character, PaletteColorSineAdd, new Vector4(0, 0, 0, 1));
			Boolean invert = EvaluationHelper.AsBoolean(character, PaletteColorInversion, false);
			Int32 basecolor = EvaluationHelper.AsInt32(character, PaletteColor, 255);

			if (time < -1) return;

			Combat.PaletteFx palfx = character.Engine.Stage.PaletteFx;
			palfx.Set(time, paladd, palmul, sinadd, invert, basecolor / 255.0f);
		}

		public Evaluation.Expression Time
		{
			get { return m_time; }
		}

		public Evaluation.Expression PaletteColorAdd
		{
			get { return m_paladd; }
		}

		public Evaluation.Expression PaletteColorMultiply
		{
			get { return m_palmul; }
		}

		public Evaluation.Expression PaletteColorSineAdd
		{
			get { return m_sineadd; }
		}

		public Evaluation.Expression PaletteColorInversion
		{
			get { return m_palinvert; }
		}

		public Evaluation.Expression PaletteColor
		{
			get { return m_palcolor; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_paladd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palmul;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_sineadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palinvert;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palcolor;

		#endregion
	}
}