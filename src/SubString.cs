using System;
using System.Diagnostics;
using System.Text;

namespace xnaMugen
{
	struct StringBuilderSubString
	{
		public static StringBuilderSubString BuildLine(StringBuilder basestring, Int32 startindex)
		{
			if (basestring == null) throw new ArgumentNullException("basestring");
			if (startindex < 0 || startindex >= basestring.Length) throw new ArgumentOutOfRangeException("startindex", "StartIndex must be greater than zero and less than the length of the basestring.");

			StringBuilderSubString substring = new StringBuilderSubString(basestring);
			substring.StartIndex = startindex;
			substring.EndIndex = FindEndlineIndex(basestring, startindex);

			return substring;
		}

		static Int32 FindEndlineIndex(StringBuilder basestring, Int32 startindex)
		{
			if (basestring == null) throw new ArgumentNullException("basestring");
			if (startindex < 0 || startindex >= basestring.Length) throw new ArgumentOutOfRangeException("startindex", "StartIndex must be greater than zero and less than the length of the basestring.");

			for (Int32 i = startindex; i < basestring.Length; ++i) if (basestring[i] == '\n') return i;

			return basestring.Length - 1;
		}

		[DebuggerStepThrough]
		public StringBuilderSubString(StringBuilder basestring)
		{
			m_basestring = basestring;
			m_startindex = 0;
			m_endindex = 0;
		}

		public void Clean()
		{
			//Eat starting spaces
			Int32 startspace = 0;
			while (startspace <= Length && Char.IsWhiteSpace(this[startspace]) == true) ++startspace;
			StartIndex += startspace;

			//Eat trailing comment
			Boolean inquote = false;
			Int32 commentindex = 0;
			for (; commentindex <= Length; ++commentindex)
			{
				Char c = this[commentindex];
				if (c == ';' && inquote == false) break;
				else if (c == '"') inquote = !inquote;
			}
			EndIndex = StartIndex + commentindex;

			//Eat ending spaces
			Int32 endspace = Length;
			while (endspace > 1 && Char.IsWhiteSpace(this[endspace - 1]) == true) --endspace;
			EndIndex = StartIndex + endspace;
		}

		public Boolean Parse(out StringBuilderSubString key, out StringBuilderSubString value)
		{
			Int32 separator_index = FindEqualsSignIndex();
			if (separator_index == -1)
			{
				key = new StringBuilderSubString();
				value = new StringBuilderSubString();
				return false;
			}

			key = new StringBuilderSubString(m_basestring);
			key.StartIndex = StartIndex;
			key.EndIndex = key.StartIndex + separator_index;
			while (key.Length > 0 && Char.IsWhiteSpace(key[key.Length - 1]) == true) --key.EndIndex;

			value = new StringBuilderSubString(m_basestring);
			value.StartIndex = StartIndex + separator_index + 1;
			value.EndIndex = EndIndex;
			while (value.Length > 0 && Char.IsWhiteSpace(value[0]) == true) ++value.StartIndex;
			if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"') { ++value.StartIndex; --value.EndIndex; }

			return true;
		}

		Int32 FindEqualsSignIndex()
		{
			for (Int32 i = 0; i < Length; ++i)
			{
				if (this[i] == '=') return i;
			}

			return -1;
		}

		[DebuggerStepThrough]
		public override String ToString()
		{
			if (m_basestring == null) return String.Empty;

			if (StartIndex < 0 || StartIndex >= m_basestring.Length || Length < 0) return String.Empty;
			return m_basestring.ToString(StartIndex, Length);
		}

		public Char this[Int32 index]
		{
			get { return (m_basestring != null) ? m_basestring[StartIndex + index] : (Char)0; }
		}

		public Int32 StartIndex
		{
			get { return m_startindex; }

			set { m_startindex = value; }
		}

		public Int32 EndIndex
		{
			get { return m_endindex; }

			set { m_endindex = value; }
		}

		public Int32 Length
		{
			get { return m_endindex - m_startindex; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StringBuilder m_basestring;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_startindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_endindex;

		#endregion
	}

	struct StringSubString
	{
		[DebuggerStepThrough]
		public StringSubString(String basestring, Int32 startindex, Int32 endindex)
		{
			m_basestring = basestring;
			m_startindex = startindex;
			m_endindex = endindex;
		}

		public void Clean()
		{
			//Eat starting spaces
			Int32 startspace = 0;
			while (startspace <= Length && Char.IsWhiteSpace(this[startspace]) == true) ++startspace;
			StartIndex += startspace;

			//Eat trailing comment
			Boolean inquote = false;
			Int32 commentindex = 0;
			for (; commentindex <= Length; ++commentindex)
			{
				Char c = this[commentindex];
				if (c == ';' && inquote == false) break;
				else if (c == '"') inquote = !inquote;
			}
			EndIndex = StartIndex + commentindex;

			//Eat ending spaces
			Int32 endspace = Length;
			while (endspace > 1 && Char.IsWhiteSpace(this[endspace - 1]) == true) --endspace;
			EndIndex = StartIndex + endspace;
		}

		public Boolean Parse(out StringSubString key, out StringSubString value)
		{
			Int32 separator_index = FindEqualsSignIndex();
			if (separator_index == -1)
			{
				key = new StringSubString();
				value = new StringSubString();
				return false;
			}

			key = new StringSubString(m_basestring, StartIndex, EndIndex);
			key.StartIndex = StartIndex;
			key.EndIndex = key.StartIndex + separator_index;
			while (key.Length > 0 && Char.IsWhiteSpace(key[key.Length - 1]) == true) --key.EndIndex;

			value = new StringSubString(m_basestring, StartIndex, EndIndex);
			value.StartIndex = StartIndex + separator_index + 1;
			value.EndIndex = EndIndex;
			while (value.Length > 0 && Char.IsWhiteSpace(value[0]) == true) ++value.StartIndex;
			if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"') { ++value.StartIndex; --value.EndIndex; }

			return true;
		}

		Int32 FindEqualsSignIndex()
		{
			for (Int32 i = 0; i < Length; ++i)
			{
				if (this[i] == '=') return i;
			}

			return -1;
		}

		public void TrimWhitespace()
		{
			while (Length > 0 && Char.IsWhiteSpace(this[0]) == true) ++StartIndex;
			while (Length > 0 && Char.IsWhiteSpace(this[Length - 1]) == true) --EndIndex;
		}

		[DebuggerStepThrough]
		public override String ToString()
		{
			if (m_basestring == null) return String.Empty;

			if (StartIndex < 0 || StartIndex >= m_basestring.Length || Length < 0) return String.Empty;
			return m_basestring.Substring(StartIndex, Length);
		}

		public Char this[Int32 index]
		{
			get { return (m_basestring != null) ? m_basestring[StartIndex + index] : (Char)0; }
		}

		public Int32 StartIndex
		{
			get { return m_startindex; }

			set { m_startindex = value; }
		}

		public Int32 EndIndex
		{
			get { return m_endindex; }

			set { m_endindex = value; }
		}

		public Int32 Length
		{
			get { return m_endindex - m_startindex; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_basestring;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_startindex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_endindex;

		#endregion
	}
}