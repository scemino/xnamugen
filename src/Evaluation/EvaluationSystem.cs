using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using xnaMugen.Collections;

namespace xnaMugen.Evaluation
{
	class EvaluationSystem : SubSystem
	{
		public EvaluationSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_expressioncache = new KeyedCollection<String, Expression>(x => x.ToString(), StringComparer.OrdinalIgnoreCase);
			m_tokenizer = new Tokenizer();
			m_treebuilder = new TreeBuilder(this);
			m_treecompiler = new TreeCompiler();
		}

		public Expression CreateExpression(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			Expression expression = BuildExpression(input);

			if (expression.IsValid == false)
			{
				Log.Write(LogLevel.Warning, LogSystem.EvaluationSystem, "Error parsing line: {0}", input);
			}

			return expression;
		}

		public PrefixedExpression CreatePrefixedExpression(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			Expression with = BuildExpression(input);
			Char prefix = input[0];
			Expression without = BuildExpression(input.Substring(1));

			Boolean? common = null;
			Expression expression = null;

			if (prefix == 's' || prefix == 'S')
			{
				if (without.IsValid == true)
				{
					common = false;
					expression = without;
				}
				else
				{
					common = false;
					expression = with;
				}
			}
			else if (prefix == 'f' || prefix == 'F')
			{
				if (without.IsValid == true)
				{
					common = true;
					expression = without;
				}
				else
				{
					common = false;
					expression = with;
				}
			}
			else
			{
				common = null;
				expression = with;
			}

			if (expression.IsValid == false)
			{
				Log.Write(LogLevel.Warning, LogSystem.EvaluationSystem, "Error parsing line: {0}", input);
			}

			return new PrefixedExpression(expression, common);
		}

		Expression BuildExpression(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			//Quoted strings must be case sensitive & cache is not case sensitive
			if (input.IndexOf('"') == -1 && m_expressioncache.Contains(input) == true) return m_expressioncache[input];

			List<Token> tokens = m_tokenizer.Tokenize(input);
			List<Node> nodes = m_treebuilder.BuildTree(tokens);

			List<EvaluationCallback> functions = new List<EvaluationCallback>();
			foreach (Node node in nodes) functions.Add(m_treecompiler.Create(node));

			Expression expression = new Expression(input, functions);

			if (input.IndexOf('"') == -1) m_expressioncache.Add(expression);

			return expression;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Tokenizer m_tokenizer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TreeBuilder m_treebuilder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TreeCompiler m_treecompiler;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly KeyedCollection<String, Expression> m_expressioncache;

		#endregion
	}

	interface IFunction
	{
		Number Evaluate(Object state);
	}
}