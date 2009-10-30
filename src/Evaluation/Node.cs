using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Evaluation
{
	class Node
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

		public static Node EmptyNode
		{
			get { return s_emptynode; }
		}

		public static Node ZeroNode
		{
			get { return s_zeronode; }
		}

		public static Node NegativeOneNode
		{
			get { return s_negativeonenode; }
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

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static Node s_emptynode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static Node s_zeronode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly static Node s_negativeonenode;

		#endregion
	}
}