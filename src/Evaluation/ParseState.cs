using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Evaluation
{
	internal class ParseState
	{
		public ParseState(EvaluationSystem system, NodeBuild fullbuild, NodeBuild endbuild, NodeBuild rangebuild, Node node, List<Token> tokens, int index)
		{
			if (system == null) throw new ArgumentNullException(nameof(system));
			if (fullbuild == null) throw new ArgumentNullException(nameof(fullbuild));
			if (endbuild == null) throw new ArgumentNullException(nameof(endbuild));
			if (rangebuild == null) throw new ArgumentNullException(nameof(rangebuild));
			if (node == null) throw new ArgumentNullException(nameof(node));
			if (tokens == null) throw new ArgumentNullException(nameof(tokens));

			m_system = system;
			m_fullnodebuild = fullbuild;
			m_endnodebuild = endbuild;
			m_rangenodebuild = rangebuild;
			m_node = node;
			m_tokens = tokens;
			m_initindex = index;
			m_currentindex = index;
		}

		public Node BuildNode(bool full)
		{
			var index = TokenIndex;
			Node node;

			if (full)
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

		public Node BuildParenNumberNode(bool frombase)
		{
			var savedindex = TokenIndex;

			if (CurrentSymbol != Symbol.LeftParen) { TokenIndex = savedindex; return null; }
			++TokenIndex;

			var node = BuildNode(true);
			if (node == null) { TokenIndex = savedindex; return null; }

			if (CurrentSymbol != Symbol.RightParen) { TokenIndex = savedindex; return null; }
			++TokenIndex;

			if (frombase)
			{
				BaseNode.Children.Add(node);
				return BaseNode;
			}

			return node;
		}

		public Node BuildRangeNode()
		{
			var index = TokenIndex;
			var node = m_rangenodebuild(m_tokens, ref index);

			TokenIndex = index;
			return node;
		}

		public T ConvertCurrentToken<T>()
		{
			return m_system.GetSubSystem<StringConverter>().Convert<T>(CurrentUnknown);
		}

		public Node BaseNode => m_node;

		public Token CurrentToken => m_currentindex >= 0 && m_currentindex < m_tokens.Count ? m_tokens[m_currentindex] : null;

		public Symbol CurrentSymbol => CurrentToken != null ? CurrentToken.AsSymbol : Symbol.None;

		public Operator CurrentOperator => CurrentToken != null ? CurrentToken.AsOperator : Operator.None;

		public string CurrentText => CurrentToken != null ? CurrentToken.AsText : null;

		public string CurrentUnknown => CurrentToken != null ? CurrentToken.AsUnknown : null;

		public int TokenIndex
		{
			get => m_currentindex;

			set { m_currentindex = value; }
		}

		public int InitialTokenIndex => m_initindex;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly EvaluationSystem m_system;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly NodeBuild m_fullnodebuild;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly NodeBuild m_endnodebuild;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly NodeBuild m_rangenodebuild;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Node m_node;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<Token> m_tokens;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_initindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_currentindex;

		#endregion
	}
}