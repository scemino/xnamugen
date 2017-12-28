using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitFallSet")]
	class HitFallSet : StateController
	{
		public HitFallSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_fallset = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_velx = textsection.GetAttribute<Evaluation.Expression>("xvel", null);
			m_vely = textsection.GetAttribute<Evaluation.Expression>("yvel", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32 fallset = EvaluationHelper.AsInt32(character, FallSet, -1);
			Single? velx = EvaluationHelper.AsSingle(character, XVelocity, null);
			Single? vely = EvaluationHelper.AsSingle(character, YVelocity, null);

			if (fallset == 0) character.DefensiveInfo.HitDef.Fall = false;
			else if (fallset == 1) character.DefensiveInfo.HitDef.Fall = true;

			if (velx != null) character.DefensiveInfo.HitDef.FallVelocityX = velx.Value;
			if (vely != null) character.DefensiveInfo.HitDef.FallVelocityY = vely.Value;
		}

		public Evaluation.Expression FallSet
		{
			get { return m_fallset; }
		}

		public Evaluation.Expression XVelocity
		{
			get { return m_velx; }
		}

		public Evaluation.Expression YVelocity
		{
			get { return m_vely; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_fallset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_velx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_vely;

		#endregion
	}
}