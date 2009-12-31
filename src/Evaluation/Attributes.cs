using System;
using System.Diagnostics;

namespace xnaMugen.Evaluation
{
	class TagAttribute : Attribute
	{
		public TagAttribute(String text)
		{
			m_text = text;
		}

		public override String ToString()
		{
			return m_text;
		}

		public String Value
		{
			get { return m_text; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_text;

		#endregion
	}

	class TokenMappingAttribute : TagAttribute
	{
		public TokenMappingAttribute(String text)
			: base(text)
		{
		}
	}

	abstract class FunctionMappingAttribute : TokenMappingAttribute
	{
		protected FunctionMappingAttribute(String text, String name)
			: base(text)
		{
			m_name = name;
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

	class UnaryOperatorMappingAttribute : FunctionMappingAttribute
	{
		public UnaryOperatorMappingAttribute(String text, String name)
			: base(text, name)
		{
		}
	}

	class BinaryOperatorMappingAttribute : FunctionMappingAttribute
	{
		public BinaryOperatorMappingAttribute(String text, String name, Int32 precedence)
			: base(text, name)
		{
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

	class CustomFunctionAttribute : FunctionMappingAttribute
	{
		public CustomFunctionAttribute(String text)
			: base(text, text)
		{
		}
	}

	class StateRedirectionAttribute : FunctionMappingAttribute
	{
		public StateRedirectionAttribute(String text)
			: base(text, text)
		{
		}
	}
}