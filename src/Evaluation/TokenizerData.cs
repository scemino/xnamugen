using System;
using System.Globalization;
using System.Diagnostics;

namespace xnaMugen.Evaluation
{
	abstract class TokenData
	{
		protected TokenData(Converter<String, Boolean> matcher)
		{
			if (matcher == null) throw new ArgumentNullException("matcher");

			m_matcher = matcher;
		}

		public Boolean Match(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			return m_matcher(input);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Converter<String, Boolean> m_matcher;

		#endregion
	}
}

namespace xnaMugen.Evaluation.Tokenizing
{
	abstract class LiteralData : TokenData
	{
		protected LiteralData(String text)
			: base(x => IsTextMatch(x, text))
		{
			if (text == null) throw new ArgumentNullException("text");

			m_text = text;
		}

		static Boolean IsTextMatch(String lhs, String rhs)
		{
			if (lhs == null) throw new ArgumentNullException("lhs");
			if (rhs == null) throw new ArgumentNullException("rhs");

			return String.Equals(lhs, rhs, StringComparison.OrdinalIgnoreCase);
		}

		public String Text
		{
			get { return m_text; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_text;

		#endregion
	}

	class UnknownData : TokenData
	{
		public UnknownData()
			: base(x => true)
		{
		}
	}

	class TextData : TokenData
	{
		public TextData()
			: base(DoMatch)
		{
		}

		static Boolean DoMatch(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			if (input.Length < 1) return false;

			return input[0] == '"' && input[input.Length - 1] == '"';
		}
	}

	class SymbolData : LiteralData
	{
		public SymbolData(Symbol symbol, String text)
			: base(text)
		{
			if (symbol == Symbol.None) throw new ArgumentOutOfRangeException("symbol");

			m_symbol = symbol;
		}

		public Symbol Symbol
		{
			get { return m_symbol; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Symbol m_symbol;

		#endregion
	}

	abstract class NodeData : LiteralData
	{
		protected NodeData(String text, String functionname)
			: base(text)
		{
			if (functionname == null) throw new ArgumentNullException("functionname");

			m_name = functionname;
		}

		public String Name
		{
			get { return m_name; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_name;

		#endregion
	}

	abstract class NumberData : TokenData
	{
		protected NumberData(Converter<String, Boolean> matcher)
			: base(matcher)
		{
		}

		public abstract Number GetNumber(String text);
	}

	class IntData : NumberData
	{
		public IntData()
			: base(IsIntNumber)
		{
		}

		public override Number GetNumber(String text)
		{
			if (text == null) throw new ArgumentNullException("text");

			Int32 number;
			if (Int32.TryParse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out number) == true) return new Number(number);

			return new Number();
		}

		static Boolean IsIntNumber(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			Int32 number;
			return Int32.TryParse(input, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out number);
		}
	}

	class FloatData : NumberData
	{
		public FloatData()
			: base(IsFloatNumber)
		{
		}

		public override Number GetNumber(String text)
		{
			if (text == null) throw new ArgumentNullException("text");

			Single number;
			if (Single.TryParse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out number) == true) return new Number(number);

			return new Number();
		}

		static Boolean IsFloatNumber(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

			Single number;
			return Single.TryParse(input, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out number);
		}
	}

	abstract class OperatorData : NodeData
	{
		protected OperatorData(Operator @operator, String text, String functionname)
			: base(text, functionname)
		{
			if (@operator == Operator.None) throw new ArgumentOutOfRangeException("@operator");

			m_operator = @operator;
		}

		public Operator Operator
		{
			get { return m_operator; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Operator m_operator;

		#endregion
	}

	class UnaryOperatorData : OperatorData
	{
		public UnaryOperatorData(Operator @operator, String text, String functionname)
			: base(@operator, text, functionname)
		{
		}
	}

	class BinaryOperatorData : OperatorData
	{
		public BinaryOperatorData(Operator @operator, String text, Int32 precedence, String functionname)
			: base(@operator, text, functionname)
		{
			if (precedence < 0) throw new ArgumentOutOfRangeException("precedence");

			m_precedence = precedence;
		}

		public Int32 Precedence
		{
			get { return m_precedence; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_precedence;

		#endregion
	}

	class CustomFunctionData : NodeData
	{
		public CustomFunctionData(String text, String functionname)
			: base(text, functionname)
		{
			var methodinfo = Type.GetType(functionname).GetMethod("Parse");

			m_function = (NodeParse)Delegate.CreateDelegate(typeof(NodeParse), methodinfo);
		}

		public Node Parse(ParseState state)
		{
			if (state == null) throw new ArgumentNullException("state");

			return m_function(state);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly NodeParse m_function;

		#endregion
	}


	class RangeData : TokenData
	{
		public RangeData()
			: base(x => false)
		{
		}
	}
}