using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Evaluation
{
	class ParseState
	{
		public ParseState(EvaluationSystem system, NodeBuild fullbuild, NodeBuild endbuild, NodeBuild rangebuild, Node node, List<Token> tokens, Int32 index)
		{
			if (system == null) throw new ArgumentNullException("system");
			if (fullbuild == null) throw new ArgumentNullException("fullbuild");
			if (endbuild == null) throw new ArgumentNullException("endbuild");
			if (rangebuild == null) throw new ArgumentNullException("rangebuild");
			if (node == null) throw new ArgumentNullException("node");
			if (tokens == null) throw new ArgumentNullException("tokens");

			m_system = system;
			m_fullnodebuild = fullbuild;
			m_endnodebuild = endbuild;
			m_rangenodebuild = rangebuild;
			m_node = node;
			m_tokens = tokens;
			m_initindex = index;
			m_currentindex = index;
		}

		public Node BuildNode(Boolean full)
		{
			Int32 index = TokenIndex;
			Node node = null;

			if (full == true)
			{
				node = m_fullnodebuild(m_tokens, ref index);
			}
			else
			{
				node = m_endnodebuild(m_tokens, ref index);
			}

			TokenIndex = index;
			return node;
		}

		public Node BuildParenNumberNode(Boolean frombase)
		{
			Int32 savedindex = TokenIndex;

			if (CurrentSymbol != Symbol.LeftParen) { TokenIndex = savedindex; return null; }
			++TokenIndex;

			Node node = BuildNode(true);
			if (node == null) { TokenIndex = savedindex; return null; }

			if (CurrentSymbol != Symbol.RightParen) { TokenIndex = savedindex; return null; }
			++TokenIndex;

			if (frombase == true)
			{
				BaseNode.Children.Add(node);
				return BaseNode;
			}
			else
			{
				return node;
			}
		}

		public Node BuildRangeNode()
		{
			Int32 index = TokenIndex;
			Node node = m_rangenodebuild(m_tokens, ref index);

			TokenIndex = index;
			return node;
		}

		public T ConvertCurrentToken<T>()
		{
			return m_system.GetSubSystem<StringConverter>().Convert<T>(CurrentUnknown);
		}

		public Node BaseNode
		{
			get { return m_node; }
		}

		public Token CurrentToken
		{
			get { return (m_currentindex >= 0 && m_currentindex < m_tokens.Count) ? m_tokens[m_currentindex] : null; }
		}

		public Symbol CurrentSymbol
		{
			get { return CurrentToken != null ? CurrentToken.AsSymbol : Symbol.None; }
		}

		public Operator CurrentOperator
		{
			get { return CurrentToken != null ? CurrentToken.AsOperator : Operator.None; }
		}

		public String CurrentText
		{
			get { return CurrentToken != null ? CurrentToken.AsText : null; }
		}

		public String CurrentUnknown
		{
			get { return CurrentToken != null ? CurrentToken.AsUnknown : null; }
		}

		public Int32 TokenIndex
		{
			get { return m_currentindex; }

			set { m_currentindex = value; }
		}

		public Int32 InitialTokenIndex
		{
			get { return m_initindex; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly EvaluationSystem m_system;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly NodeBuild m_fullnodebuild;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly NodeBuild m_endnodebuild;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly NodeBuild m_rangenodebuild;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Node m_node;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Token> m_tokens;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_initindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_currentindex;

		#endregion
	}
}