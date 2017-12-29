using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Evaluation
{
	internal class TreeBuilder
	{
		public TreeBuilder(EvaluationSystem system)
		{
			if (system == null) throw new ArgumentNullException(nameof(system));

			m_system = system;
			m_fullnodebuild = ParseNode;
			m_endnodebuild = ParseEndNode;
			m_rangenodebuild = ParseRangeNode;
		}

		public List<Node> BuildTree(List<Token> tokens)
		{
			if (tokens == null) throw new ArgumentNullException(nameof(tokens));

			var tokenindex = 0;
			var output = new List<Node>();

			while (GetToken(tokens, tokenindex) != null)
			{
				var node = ParseNode(tokens, ref tokenindex);
				if (node == null)
				{
					output.Clear();
					return output;
				}

				output.Add(node);

				if (GetToken(tokens, tokenindex) != null)
				{
					if (GetToken(tokens, tokenindex).AsSymbol == Symbol.Comma)
					{
						++tokenindex;
					}
					else
					{
						output.Clear();
						return output;
					}
				}
			}

			return output;
		}

		private Node ParseNode(List<Token> tokens, ref int tokenindex)
		{
			if (tokens == null) throw new ArgumentNullException(nameof(tokens));
			if (tokenindex < 0 || tokenindex >= tokens.Count) throw new ArgumentOutOfRangeException(nameof(tokenindex));

			var lhs = ParseEndNode(tokens, ref tokenindex);
			if (lhs == null) return null;

			for (var token = GetToken(tokens, tokenindex); token != null; token = GetToken(tokens, tokenindex))
			{
				if (token.Data is Tokenizing.BinaryOperatorData == false) break;

				if (lhs.Token.Data is Tokenizing.RangeData && lhs.PrecedenceOverride == false) return null;

				var operatornode = new Node(token);
				++tokenindex;

				var @operator = (token.Data as Tokenizing.BinaryOperatorData).Operator;

				if (@operator == Operator.Equals || @operator == Operator.NotEquals)
				{
					var range = ParseRangeNode(tokens, ref tokenindex);
					if (range != null)
					{
						range.Children[0] = lhs;
						range.Arguments[0] = @operator;

						lhs = range;
						continue;
					}
				}

				var rhs = ParseNode(tokens, ref tokenindex);
				if (rhs == null) return null;

				if(SwitchOrder(operatornode, rhs))
				{
					lhs = BreakTree(lhs, operatornode, rhs);
				}
				else
				{
					operatornode.Children.Add(lhs);
					operatornode.Children.Add(rhs);

					lhs = operatornode;
				}
			}

			return lhs;
		}

		private Node ParseEndNode(List<Token> tokens, ref int tokenindex)
		{
			if (tokens == null) throw new ArgumentNullException(nameof(tokens));
			if (tokenindex < 0 || tokenindex >= tokens.Count) throw new ArgumentOutOfRangeException(nameof(tokenindex));

			var token = GetToken(tokens, tokenindex);
			if (token == null) return null;

			if (token.Data is Tokenizing.NumberData)
			{
				++tokenindex;

				var node = new Node(token);
				return node;
			}

			if (token.Data is Tokenizing.UnaryOperatorData)
			{
				++tokenindex;

				var node = new Node(token);

				var child = ParseEndNode(tokens, ref tokenindex);
				if (child == null) return null;

				node.Children.Add(child);
				return node;
			}

			if (token.AsOperator == Operator.Minus || token.AsOperator == Operator.Plus)
			{
				++tokenindex;

				var node = new Node(token);

				var child = ParseEndNode(tokens, ref tokenindex);
				if (child == null) return null;

				node.Children.Add(child);
				return node;
			}

			if (token.AsSymbol == Symbol.LeftParen)
			{
				var savedindex = tokenindex;

				++tokenindex;

				var node = ParseNode(tokens, ref tokenindex);
				if (node != null)
				{
					var endtoken = GetToken(tokens, tokenindex);
					if (endtoken != null && endtoken.AsSymbol == Symbol.RightParen)
					{
						++tokenindex;
						node.PrecedenceOverride = true;
						return node;
					}
				}

				tokenindex = savedindex;
			}

			if (token.Data is Tokenizing.CustomFunctionData)
			{
				var data = token.Data as Tokenizing.CustomFunctionData;

				var node = new Node(token);
				++tokenindex;

				var state = new ParseState(m_system, m_fullnodebuild, m_endnodebuild, m_rangenodebuild, node, tokens, tokenindex);

				var parsednode = data.Parse(state);
				if (parsednode != null)
				{
					tokenindex = state.TokenIndex;
					return parsednode;
				}

				tokenindex = state.InitialTokenIndex - 1;
				return null;
			}

			if (token.Data is Tokenizing.StateRedirectionData)
			{
				var data = token.Data as Tokenizing.StateRedirectionData;

				var node = new Node(token);
				++tokenindex;

				var state = new ParseState(m_system, m_fullnodebuild, m_endnodebuild, m_rangenodebuild, node, tokens, tokenindex);

				var parsednode = data.Parse(state);
				if (parsednode != null)
				{
					tokenindex = state.TokenIndex;
					return parsednode;
				}

				tokenindex = state.InitialTokenIndex - 1;
				return null;
			}

			return null;
		}

		private Node ParseRangeNode(List<Token> tokens, ref int tokenindex)
		{
			if (tokens == null) throw new ArgumentNullException(nameof(tokens));
			if (tokenindex < 0 || tokenindex >= tokens.Count) throw new ArgumentOutOfRangeException(nameof(tokenindex));

			var savedindex = tokenindex;

			var tokenPre = GetToken(tokens, tokenindex);
			if (tokenPre == null || tokenPre.AsSymbol != Symbol.LeftBracket && tokenPre.AsSymbol != Symbol.LeftParen) goto EndOfMethod;
			++tokenindex;

			var nodeLeft = ParseNode(tokens, ref tokenindex);
			if (nodeLeft == null) goto EndOfMethod;

			var tokenComma = GetToken(tokens, tokenindex);
			if (tokenComma == null || tokenComma.AsSymbol != Symbol.Comma) goto EndOfMethod;
			++tokenindex;

			var nodeRight = ParseNode(tokens, ref tokenindex);
			if (nodeRight == null) goto EndOfMethod;

			var tokenPost = GetToken(tokens, tokenindex);
			if (tokenPost == null || tokenPost.AsSymbol != Symbol.RightBracket && tokenPost.AsSymbol != Symbol.RightParen) goto EndOfMethod;
			++tokenindex;

			var rangenode = new Node(new Token(string.Empty, new Tokenizing.RangeData()));
			rangenode.Children.Add(null);
			rangenode.Children.Add(nodeLeft);
			rangenode.Children.Add(nodeRight);
			rangenode.Arguments.Add(null);
			rangenode.Arguments.Add(tokenPre.AsSymbol);
			rangenode.Arguments.Add(tokenPost.AsSymbol);
			return rangenode;

		EndOfMethod:
			tokenindex = savedindex;
			return null;
		}

		private static Node BreakTree(Node lhs, Node operatornode, Node rhs)
		{
			if (lhs == null) throw new ArgumentNullException(nameof(lhs));
			if (operatornode == null) throw new ArgumentNullException(nameof(operatornode));
			if (rhs == null) throw new ArgumentNullException(nameof(rhs));

			operatornode.Children.Add(lhs);

			var newbase = rhs;
			while (newbase.Children.Count != 0 && newbase.Children[0].Children.Count != 0 && newbase.Children[0].PrecedenceOverride == false && SwitchOrder(operatornode, newbase.Children[0]))
			{
				newbase = newbase.Children[0];
			}

			operatornode.Children.Add(newbase.Children[0]);
			newbase.Children[0] = operatornode;

			return rhs;
		}

		private static Token GetToken(List<Token> tokens, int index)
		{
			if (tokens == null) throw new ArgumentNullException(nameof(tokens));

			return index >= 0 && index < tokens.Count ? tokens[index] : null;
		}

		private static bool SwitchOrder(Node lhs, Node rhs)
		{
			if (lhs == null) throw new ArgumentNullException(nameof(lhs));
			if (rhs == null) throw new ArgumentNullException(nameof(rhs));

			if (rhs.PrecedenceOverride) return false;

			var lhsdata = lhs.Token.Data as Tokenizing.BinaryOperatorData;
			var rhsdata = rhs.Token.Data as Tokenizing.BinaryOperatorData;
			if (lhsdata == null || rhsdata == null) return false;

			return lhsdata.Precedence >= rhsdata.Precedence;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly EvaluationSystem m_system;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly NodeBuild m_fullnodebuild;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly NodeBuild m_endnodebuild;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly NodeBuild m_rangenodebuild;

		#endregion
	}
}