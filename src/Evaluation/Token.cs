using System;
using System.Diagnostics;

namespace xnaMugen.Evaluation
{
	internal class Token
	{
		public Token(string text, TokenData data)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));
			if (data == null) throw new ArgumentNullException(nameof(data));

			m_text = text;
			m_data = data;
		}

		public override string ToString()
		{
			return m_text;
		}

		public TokenData Data => m_data;

		public Symbol AsSymbol
		{
			get
			{
				var data = Data as Tokenizing.SymbolData;
				if (data == null) return Symbol.None;

				return data.Symbol;
			}
		}

		public Operator AsOperator
		{
			get
			{
				var data = Data as Tokenizing.OperatorData;
				if (data == null) return Operator.None;

				return data.Operator;
			}
		}

		public string AsText
		{
			get
			{
				var data = Data as Tokenizing.TextData;
				if (data == null) return null;

				return m_text.Substring(1, m_text.Length - 2);
			}
		}

		public string AsUnknown
		{
			get
			{
				var data = Data as Tokenizing.UnknownData;
				if (data == null) return null;

				return m_text;
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_text;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly TokenData m_data;

		#endregion
	}
}