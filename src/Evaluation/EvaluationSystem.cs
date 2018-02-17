using System;
using System.Collections.Generic;
using System.Diagnostics;
using xnaMugen.Collections;

namespace xnaMugen.Evaluation
{
	internal class EvaluationException : Exception { }

	internal class EvaluationSystem : SubSystem
	{
		public EvaluationSystem(SubSystems subsystems)
			: base(subsystems)
		{
			m_expressioncache = new KeyedCollection<string, Expression>(x => x.ToString(), StringComparer.OrdinalIgnoreCase);
			m_tokenizer = new Tokenizer();
			m_treebuilder = new TreeBuilder(this);

            //m_compiler = new ILCompiler();
            m_compiler = new ExpressionCompiler();

			//var exp = CreateExpression("0.0 = [-10, 10]");
			//var result = exp.Evaluate(null);
		}

		public Expression CreateExpression(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			var expression = BuildExpression(input);

			if (expression.IsValid == false)
			{
				Log.Write(LogLevel.Warning, LogSystem.EvaluationSystem, "Error parsing line: {0}", input);
			}

			return expression;
		}

		public PrefixedExpression CreatePrefixedExpression(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			var with = BuildExpression(input);
			var prefix = input[0];
			var without = BuildExpression(input.Substring(1));

			bool? common;
			Expression expression;

			if (prefix == 's' || prefix == 'S')
			{
				if (without.IsValid)
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
				if (without.IsValid)
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

		private Expression BuildExpression(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			//Quoted strings must be case sensitive & cache is not case sensitive
			if (input.IndexOf('"') == -1 && m_expressioncache.Contains(input)) return m_expressioncache[input];

			var tokens = m_tokenizer.Tokenize(input);
			var nodes = m_treebuilder.BuildTree(tokens);

			var functions = new List<EvaluationCallback>();
			foreach (var node in nodes) functions.Add(m_compiler.Create(node));

			var expression = new Expression(input, functions);

			if (input.IndexOf('"') == -1) m_expressioncache.Add(expression);

			return expression;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Tokenizer m_tokenizer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly TreeBuilder m_treebuilder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ICompiler m_compiler;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly KeyedCollection<string, Expression> m_expressioncache;

		#endregion
	}
}