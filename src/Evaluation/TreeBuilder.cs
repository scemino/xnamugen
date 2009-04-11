using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Evaluation
{
	class TreeBuilder
	{
		public TreeBuilder(EvaluationSystem system)
		{
			if (system == null) throw new ArgumentNullException("system");

			m_system = system;
			m_fullnodebuild = ParseNode;
			m_endnodebuild = ParseEndNode;
			m_rangenodebuild = ParseRangeNode;
		}

		public List<Node> BuildTree(List<Token> tokens)
		{
			if (tokens == null) throw new ArgumentNullException("tokens");

			Int32 tokenindex = 0;
			List<Node> output = new List<Node>();

			while (GetToken(tokens, tokenindex) != null)
			{
				Node node = ParseNode(tokens, ref tokenindex);
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

		Node ParseNode(List<Token> tokens, ref Int32 tokenindex)
		{
			if (tokens == null) throw new ArgumentNullException("tokens");
			if (tokenindex < 0 || tokenindex >= tokens.Count) throw new ArgumentOutOfRangeException("tokenindex");

			Node lhs = ParseEndNode(tokens, ref tokenindex);
			if (lhs == null) return null;

			for (Token token = GetToken(tokens, tokenindex); token != null; token = GetToken(tokens, tokenindex))
			{
				if ((token.Data is Tokenizing.BinaryOperatorData) == false) break;

				if (lhs.Token.Data is Tokenizing.RangeData && lhs.PrecedenceOverride == false) return null;

				Node operatornode = new Node(token);
				++tokenindex;

				Operator @operator = (token.Data as Tokenizing.BinaryOperatorData).Operator;

				if (@operator == Operator.Equals || @operator == Operator.NotEquals)
				{
					Node range = ParseRangeNode(tokens, ref tokenindex);
					if (range != null)
					{
						range.Children[0] = lhs;
						range.Arguments[0] = @operator;

						lhs = range;
						continue;
					}
				}

				Node rhs = ParseNode(tokens, ref tokenindex);
				if (rhs == null) return null;

				if(SwitchOrder(operatornode, rhs) == true)
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

		Node ParseEndNode(List<Token> tokens, ref Int32 tokenindex)
		{
			if (tokens == null) throw new ArgumentNullException("tokens");
			if (tokenindex < 0 || tokenindex >= tokens.Count) throw new ArgumentOutOfRangeException("tokenindex");

			Token token = GetToken(tokens, tokenindex);
			if (token == null) return null;

			if (token.Data is Tokenizing.NumberData)
			{
				++tokenindex;

				Node node = new Node(token);
				return node;
			}

			if (token.Data is Tokenizing.UnaryOperatorData)
			{
				++tokenindex;

				Node node = new Node(token);

				Node child = ParseEndNode(tokens, ref tokenindex);
				if (child == null) return null;

				node.Children.Add(child);
				return node;
			}

			if (token.AsOperator == Operator.Minus || token.AsOperator == Operator.Plus)
			{
				++tokenindex;

				Node node = new Node(token);

				Node child = ParseEndNode(tokens, ref tokenindex);
				if (child == null) return null;

				node.Children.Add(child);
				return node;
			}

			if (token.AsSymbol == Symbol.LeftParen)
			{
				Int32 savedindex = tokenindex;

				++tokenindex;

				Node node = ParseNode(tokens, ref tokenindex);
				if (node != null)
				{
					Token endtoken = GetToken(tokens, tokenindex);
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
				Tokenizing.CustomFunctionData data = token.Data as Tokenizing.CustomFunctionData;

				Node node = new Node(token);
				++tokenindex;

				ParseState state = new ParseState(m_system, m_fullnodebuild, m_endnodebuild, m_rangenodebuild, node, tokens, tokenindex);

				Node parsednode = data.Parse(state);
				if (parsednode != null)
				{
					tokenindex = state.TokenIndex;
					return parsednode;
				}
				else
				{
					tokenindex = state.InitialTokenIndex - 1;
					return null;
				}
			}

			return null;
		}

		Node ParseRangeNode(List<Token> tokens, ref Int32 tokenindex)
		{
			if (tokens == null) throw new ArgumentNullException("tokens");
			if (tokenindex < 0 || tokenindex >= tokens.Count) throw new ArgumentOutOfRangeException("tokenindex");

			Int32 savedindex = tokenindex;

			Token token_pre = GetToken(tokens, tokenindex);
			if (token_pre == null || (token_pre.AsSymbol != Symbol.LeftBracket && token_pre.AsSymbol != Symbol.LeftParen)) goto EndOfMethod;
			++tokenindex;

			Node node_left = ParseNode(tokens, ref tokenindex);
			if (node_left == null) goto EndOfMethod;

			Token token_comma = GetToken(tokens, tokenindex);
			if (token_comma == null || token_comma.AsSymbol != Symbol.Comma) goto EndOfMethod;
			++tokenindex;

			Node node_right = ParseNode(tokens, ref tokenindex);
			if (node_right == null) goto EndOfMethod;

			Token token_post = GetToken(tokens, tokenindex);
			if (token_post == null || (token_post.AsSymbol != Symbol.RightBracket && token_post.AsSymbol != Symbol.RightParen)) goto EndOfMethod;
			++tokenindex;

			Node rangenode = new Node(new Token(String.Empty, new Tokenizing.RangeData()));
			rangenode.Children.Add(null);
			rangenode.Children.Add(node_left);
			rangenode.Children.Add(node_right);
			rangenode.Arguments.Add(null);
			rangenode.Arguments.Add(token_pre.AsSymbol);
			rangenode.Arguments.Add(token_post.AsSymbol);
			return rangenode;

		EndOfMethod:
			tokenindex = savedindex;
			return null;
		}

		static Node BreakTree(Node lhs, Node operatornode, Node rhs)
		{
			if (lhs == null) throw new ArgumentNullException("lhs");
			if (operatornode == null) throw new ArgumentNullException("operatornode");
			if (rhs == null) throw new ArgumentNullException("rhs");

			operatornode.Children.Add(lhs);

			Node newbase = rhs;
			while (newbase.Children.Count != 0 && newbase.Children[0].Children.Count != 0 && newbase.Children[0].PrecedenceOverride == false && SwitchOrder(operatornode, newbase.Children[0]) == true)
			{
				newbase = newbase.Children[0];
			}

			operatornode.Children.Add(newbase.Children[0]);
			newbase.Children[0] = operatornode;

			return rhs;
		}

		static Token GetToken(List<Token> tokens, Int32 index)
		{
			if (tokens == null) throw new ArgumentNullException("tokens");

			return (index >= 0 && index < tokens.Count) ? tokens[index] : null;
		}

		static Boolean SwitchOrder(Node lhs, Node rhs)
		{
			if (lhs == null) throw new ArgumentNullException("lhs");
			if (rhs == null) throw new ArgumentNullException("rhs");

			if (rhs.PrecedenceOverride == true) return false;

			Tokenizing.BinaryOperatorData lhsdata = lhs.Token.Data as Tokenizing.BinaryOperatorData;
			Tokenizing.BinaryOperatorData rhsdata = rhs.Token.Data as Tokenizing.BinaryOperatorData;

			if (lhsdata == null || lhs.Children.Count != 2) return false;
			if (rhsdata == null || rhs.Children.Count != 2) return false;

			return lhsdata.Precedence >= rhsdata.Precedence;
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

		#endregion
	}
}