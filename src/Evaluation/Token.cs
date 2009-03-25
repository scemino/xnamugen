using System;
using System.Diagnostics;

namespace xnaMugen.Evaluation
{
	class Token
	{
		public Token(String text, TokenData data)
		{
			if (text == null) throw new ArgumentNullException("text");
			if (data == null) throw new ArgumentNullException("data");

			m_text = text;
			m_data = data;
		}

		public override String ToString()
		{
			return m_text;
		}

		public TokenData Data
		{
			get { return m_data; }
		}

		public Symbol AsSymbol
		{
			get
			{
				Tokenizing.SymbolData data = Data as Tokenizing.SymbolData;
				if (data == null) return Symbol.None;

				return data.Symbol;
			}
		}

		public Operator AsOperator
		{
			get
			{
				Tokenizing.OperatorData data = Data as Tokenizing.OperatorData;
				if (data == null) return Operator.None;

				return data.Operator;
			}
		}

		public String AsText
		{
			get
			{
				Tokenizing.TextData data = Data as Tokenizing.TextData;
				if (data == null) return null;

				return m_text.Substring(1, m_text.Length - 2);
			}
		}

		public String AsUnknown
		{
			get
			{
				Tokenizing.UnknownData data = Data as Tokenizing.UnknownData;
				if (data == null) return null;

				return m_text;
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_text;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TokenData m_data;

		#endregion
	}
}