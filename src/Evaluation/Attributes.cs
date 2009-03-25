using System;
using System.Diagnostics;

namespace xnaMugen.Evaluation
{
	class TokenMappingAttribute : Attribute
	{
		public TokenMappingAttribute(String text)
		{
			if (text == null) throw new ArgumentNullException("text");

			m_text = text;
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

	class FunctionMappingAttribute : TokenMappingAttribute
	{
		public FunctionMappingAttribute(String text, Type type)
			: base(text)
		{
			if (type == null) throw new ArgumentNullException("type");

			m_type = type;
		}

		public Type Type
		{
			get { return m_type; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Type m_type;

		#endregion
	}

	class BinaryOperatorMappingAttribute : FunctionMappingAttribute
	{
		public BinaryOperatorMappingAttribute(String text, Type type, Int32 precedence)
			: base(text, type)
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

	class CustomFunctionAttribute : Attribute
	{
		public CustomFunctionAttribute(String text)
		{
			if (text == null) throw new ArgumentNullException("text");

			m_text = text;
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
}