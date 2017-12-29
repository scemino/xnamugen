using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Evaluation
{
	internal class Node
	{
		static Node()
		{
			s_emptynode = new Node(new Token("", new Tokenizing.IntData()));
			s_zeronode = new Node(new Token("0", new Tokenizing.IntData()));
			s_negativeonenode = new Node(new Token("-1", new Tokenizing.IntData()));
		}

		public Node(Token token)
		{
			m_token = token;
			m_children = new List<Node>();
			m_arguments = new List<object>();
		}

		public override string ToString()
		{
			return m_token.ToString();
		}

		public Token Token => m_token;

		public List<Node> Children => m_children;

		public List<object> Arguments => m_arguments;

		public bool PrecedenceOverride
		{
			get => m_precedenceoverride;

			set { m_precedenceoverride = value; }
		}

		public static Node ZeroNode => s_zeronode;

		public static Node NegativeOneNode => s_negativeonenode;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Token m_token;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<Node> m_children;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<object> m_arguments;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_precedenceoverride;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Node s_emptynode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Node s_zeronode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Node s_negativeonenode;

		#endregion
	}
}