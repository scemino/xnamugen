using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("PlayerPush")]
	class PlayerPush : StateController
	{
		public PlayerPush(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_value = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			Boolean? pushflag = EvaluationHelper.AsBoolean(character, PushFlag, null);

			if (pushflag != null)
			{
				character.PushFlag = pushflag.Value;
			} 
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (PushFlag == null) return false;

			return true;
		}

		public Evaluation.Expression PushFlag
		{
			get { return m_value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_value;

		#endregion
	}
}