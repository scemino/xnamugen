using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AfterImage")]
	class AfterImage : StateController
	{
		public AfterImage(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_numberofframes = textsection.GetAttribute<Evaluation.Expression>("length", null);
			m_palettecolor = textsection.GetAttribute<Evaluation.Expression>("palcolor", null);
			m_paletteinversion = textsection.GetAttribute<Evaluation.Expression>("palinvertall", null);
			m_palettebrightness = textsection.GetAttribute<Evaluation.Expression>("palbright", null);
			m_palettecontrast = textsection.GetAttribute<Evaluation.Expression>("palcontrast", null);
			m_palettepostbrightness = textsection.GetAttribute<Evaluation.Expression>("palpostbright", null);
			m_paletteadd = textsection.GetAttribute<Evaluation.Expression>("paladd", null);
			m_palettemutliply = textsection.GetAttribute<Evaluation.Expression>("palmul", null);
			m_timegap = textsection.GetAttribute<Evaluation.Expression>("TimeGap", null);
			m_framegap = textsection.GetAttribute<Evaluation.Expression>("FrameGap", null);
			m_blending = textsection.GetAttribute<Blending?>("trans", null);
			m_alpha = textsection.GetAttribute<Evaluation.Expression>("alpha", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32 time = EvaluationHelper.AsInt32(character, Time, 1);
			Int32 numberofframes = EvaluationHelper.AsInt32(character, NumberOfFrames, 20);
			Int32 basecolor = EvaluationHelper.AsInt32(character, PaletteColor, 255);
			Boolean invert = EvaluationHelper.AsBoolean(character, PaletteColorInversion, false);
			Vector3 palpreadd = EvaluationHelper.AsVector3(character, PaletteColorBrightness, new Vector3(30, 30, 30));
			Vector3 palcontrast = EvaluationHelper.AsVector3(character, PaletteColorContrast, new Vector3(255, 255, 255));
			Vector3 palpostadd = EvaluationHelper.AsVector3(character, PalettePostBrightness, new Vector3(0, 0, 0));
			Vector3 paladd = EvaluationHelper.AsVector3(character, PaletteColorAdd, new Vector3(10, 10, 25));
			Vector3 palmul = EvaluationHelper.AsVector3(character, PaletteColorMultiply, new Vector3(.65f, .65f, .75f));
			Int32 timegap = EvaluationHelper.AsInt32(character, TimeGap, 1);
			Int32 framegap = EvaluationHelper.AsInt32(character, FrameGap, 4);
			Point alpha = EvaluationHelper.AsPoint(character, Alpha, new Point(255, 0));

			Blending? trans = Transparency;
			if (trans != null && trans.Value.BlendType == BlendType.Add && trans.Value.SourceFactor == 0 && trans.Value.DestinationFactor == 0) trans = new Blending(BlendType.Add, alpha.X, alpha.Y);

			Combat.AfterImage afterimages = character.AfterImages;
			afterimages.Reset();
			afterimages.DisplayTime = time;
			afterimages.NumberOfFrames = numberofframes;
			afterimages.BaseColor = basecolor / 255.0f;
			afterimages.InvertColor = invert;
			afterimages.ColorPreAdd = Vector3.Clamp(palpreadd / 255.0f, Vector3.Zero, Vector3.One);
			afterimages.ColorContrast = Vector3.Clamp(palcontrast / 255.0f, Vector3.Zero, new Vector3(Single.MaxValue));
			afterimages.ColorPostAdd = Vector3.Clamp(palpostadd / 255.0f, Vector3.Zero, Vector3.One);
			afterimages.ColorPaletteAdd = Vector3.Clamp(paladd / 255.0f, Vector3.Zero, Vector3.One);
			afterimages.ColorPaletteMultiply = Vector3.Clamp(palmul, Vector3.Zero, new Vector3(Single.MaxValue));
			afterimages.TimeGap = timegap;
			afterimages.FrameGap = framegap;
			afterimages.Transparency = trans;
			afterimages.IsActive = true;
		}

		public Evaluation.Expression Time
		{
			get { return m_time; }
		}

		public Evaluation.Expression NumberOfFrames
		{
			get { return m_numberofframes; }
		}

		public Evaluation.Expression PaletteColor
		{
			get { return m_palettecolor; }
		}

		public Evaluation.Expression PaletteColorInversion
		{
			get { return m_paletteinversion; }
		}

		public Evaluation.Expression PaletteColorBrightness
		{
			get { return m_palettebrightness; }
		}

		public Evaluation.Expression PaletteColorContrast
		{
			get { return m_palettecontrast; }
		}

		public Evaluation.Expression PalettePostBrightness
		{
			get { return m_palettepostbrightness; }
		}

		public Evaluation.Expression PaletteColorAdd
		{
			get { return m_paletteadd; }
		}

		public Evaluation.Expression PaletteColorMultiply
		{
			get { return m_palettemutliply; }
		}

		public Evaluation.Expression TimeGap
		{
			get { return m_timegap; }
		}

		public Evaluation.Expression FrameGap
		{
			get { return m_framegap; }
		}

		public Blending? Transparency
		{
			get { return m_blending; }
		}

		public Evaluation.Expression Alpha
		{
			get { return m_alpha; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_numberofframes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palettecolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_paletteinversion;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palettebrightness;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palettecontrast;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palettepostbrightness;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_paletteadd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_palettemutliply;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_timegap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_framegap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Blending? m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_alpha;

		#endregion
	}
}