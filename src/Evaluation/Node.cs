using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Evaluation
{
	class Node
	{
		public Node(Token token)
		{
			if (token == null) throw new ArgumentNullException("token");

			m_token = token;
			m_children = new List<Node>();
			m_arguments = new List<Object>();
		}

		public override String ToString()
		{
			return m_token.ToString();
		}

		public Token Token
		{
			get { return m_token; }
		}

		public List<Node> Children
		{
			get { return m_children; }
		}

		public List<Object> Arguments
		{
			get { return m_arguments; }
		}

		public Boolean PrecedenceOverride
		{
			get { return m_precedenceoverride; }

			set { m_precedenceoverride = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Token m_token;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Node> m_children;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Object> m_arguments;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_precedenceoverride;

		#endregion
	}
}