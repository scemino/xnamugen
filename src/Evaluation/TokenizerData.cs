using System;
using System.Globalization;
using System.Diagnostics;

namespace xnaMugen.Evaluation
{
	internal abstract class TokenData
	{
		protected TokenData(Converter<string, bool> matcher)
		{
			if (matcher == null) throw new ArgumentNullException(nameof(matcher));

			m_matcher = matcher;
		}

		public bool Match(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			return m_matcher(input);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Converter<string, bool> m_matcher;

		#endregion
	}
}

namespace xnaMugen.Evaluation.Tokenizing
{
	internal abstract class LiteralData : TokenData
	{
		protected LiteralData(string text)
			: base(x => IsTextMatch(x, text))
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			m_text = text;
		}

		private static bool IsTextMatch(string lhs, string rhs)
		{
			if (lhs == null) throw new ArgumentNullException(nameof(lhs));
			if (rhs == null) throw new ArgumentNullException(nameof(rhs));

			return string.Equals(lhs, rhs, StringComparison.OrdinalIgnoreCase);
		}

		public string Text => m_text;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_text;

		#endregion
	}

	internal class UnknownData : TokenData
	{
		public UnknownData()
			: base(x => true)
		{
		}
	}

	internal class TextData : TokenData
	{
		public TextData()
			: base(DoMatch)
		{
		}

		private static bool DoMatch(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			if (input.Length < 1) return false;

			return input[0] == '"' && input[input.Length - 1] == '"';
		}
	}

	internal class SymbolData : LiteralData
	{
		public SymbolData(Symbol symbol, string text)
			: base(text)
		{
			if (symbol == Symbol.None) throw new ArgumentOutOfRangeException(nameof(symbol));

			m_symbol = symbol;
		}

		public Symbol Symbol => m_symbol;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Symbol m_symbol;

		#endregion
	}

	internal abstract class NodeData : LiteralData
	{
		protected NodeData(string text, string functionname)
			: base(text)
		{
			if (functionname == null) throw new ArgumentNullException(nameof(functionname));

			m_name = functionname;
		}

		public string Name => m_name;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_name;

		#endregion
	}

	internal abstract class NumberData : TokenData
	{
		protected NumberData(Converter<string, bool> matcher)
			: base(matcher)
		{
		}

		public abstract Number GetNumber(string text);
	}

	internal class IntData : NumberData
	{
		public IntData()
			: base(IsIntNumber)
		{
		}

		public override Number GetNumber(string text)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			int number;
			if (int.TryParse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out number)) return new Number(number);

			return new Number();
		}

		private static bool IsIntNumber(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			int number;
			return int.TryParse(input, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out number);
		}
	}

	internal class FloatData : NumberData
	{
		public FloatData()
			: base(IsFloatNumber)
		{
		}

		public override Number GetNumber(string text)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			float number;
			if (float.TryParse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out number)) return new Number(number);

			return new Number();
		}

		private static bool IsFloatNumber(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			float number;
			return float.TryParse(input, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out number);
		}
	}

	internal abstract class OperatorData : NodeData
	{
		protected OperatorData(Operator @operator, string text, string functionname)
			: base(text, functionname)
		{
			if (@operator == Operator.None) throw new ArgumentOutOfRangeException("@operator");

			m_operator = @operator;
		}

		public Operator Operator => m_operator;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Operator m_operator;

		#endregion
	}

	internal class UnaryOperatorData : OperatorData
	{
		public UnaryOperatorData(Operator @operator, string text, string functionname)
			: base(@operator, text, functionname)
		{
		}
	}

	internal class BinaryOperatorData : OperatorData
	{
		public BinaryOperatorData(Operator @operator, string text, string functionname, int precedence)
			: base(@operator, text, functionname)
		{
			if (precedence < 0) throw new ArgumentOutOfRangeException(nameof(precedence));

			m_precedence = precedence;
		}

		public int Precedence => m_precedence;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_precedence;

		#endregion
	}

	internal class CustomFunctionData : NodeData
	{
		public CustomFunctionData(string text, string name, Type type)
			: base(text, name)
		{
			m_type = type;
			m_parse = (NodeParse)Delegate.CreateDelegate(typeof(NodeParse), type.GetMethod("Parse", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public));
		}

		public Type Type => m_type;

		public NodeParse Parse => m_parse;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type m_type;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly NodeParse m_parse;

		#endregion
	}

	internal class StateRedirectionData : CustomFunctionData
	{
		public StateRedirectionData(string text, string name, Type type)
			: base(text, name, type)
		{
		}
	}

	internal class RangeData : TokenData
	{
		public RangeData()
			: base(x => false)
		{
		}
	}
}