using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace xnaMugen.Evaluation
{
	class Tokenizer
	{
		public Tokenizer()
		{
            m_hackregex = new Regex(@"(ProjContact|ProjGuarded|ProjHit)(\d+)", RegexOptions.IgnoreCase);
            m_hackmatchevaluator = m => m.Groups[1].Value + "(" + m.Groups[2].Value + ")";
			m_intdata = new Tokenizing.IntData();
			m_floatdata = new Tokenizing.FloatData();
			m_textdata = new Tokenizing.TextData();
			m_unknowndata = new Tokenizing.UnknownData();
			m_tokenmap = BuildTokenMapping();
		}

		Dictionary<String, TokenData> BuildTokenMapping()
		{
			Dictionary<String, TokenData> mapping = new Dictionary<String, TokenData>(StringComparer.OrdinalIgnoreCase);

			foreach (FieldInfo field in typeof(Operator).GetFields())
			{
				BinaryOperatorMappingAttribute bom_attrib = (BinaryOperatorMappingAttribute)Attribute.GetCustomAttribute(field, typeof(BinaryOperatorMappingAttribute));
				if (bom_attrib != null)
				{
					Operator @operator = (Operator)field.GetValue(null);
					mapping.Add(bom_attrib.Value, new Tokenizing.BinaryOperatorData(@operator, bom_attrib.Value, bom_attrib.Name, bom_attrib.Precedence));
				}
				else
				{
					UnaryOperatorMappingAttribute fm_attr = (UnaryOperatorMappingAttribute)Attribute.GetCustomAttribute(field, typeof(UnaryOperatorMappingAttribute));
					if (fm_attr != null)
					{
						Operator @operator = (Operator)field.GetValue(null);
						mapping.Add(fm_attr.Value, new Tokenizing.UnaryOperatorData(@operator, fm_attr.Value, fm_attr.Name));
					}
				}
			}

			foreach (FieldInfo field in typeof(Symbol).GetFields())
			{
				TokenMappingAttribute attr = (TokenMappingAttribute)Attribute.GetCustomAttribute(field, typeof(TokenMappingAttribute));
				if (attr != null)
				{
					Symbol symbol = (Symbol)field.GetValue(null);
					mapping.Add(attr.Value, new Tokenizing.SymbolData(symbol, attr.Value));
				}
			}

			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsAbstract == false || type.IsClass == false) continue;

				CustomFunctionAttribute attr = (CustomFunctionAttribute)Attribute.GetCustomAttribute(type, typeof(CustomFunctionAttribute));
				if (attr == null) continue;

				mapping.Add(attr.Value, new Tokenizing.CustomFunctionData(attr.Value, attr.Value, type));
			}

			return mapping;
		}

		public List<Token> Tokenize(String input)
		{
			if (input == null) throw new ArgumentNullException("input");

            input = m_hackregex.Replace(input, m_hackmatchevaluator);

			List<Token> output = new List<Token>();
			Int32 index = 0;

			while (true)
			{
				while (index < input.Length && Char.IsWhiteSpace(input[index]) == true) ++index;
				if (index >= input.Length) break;

				Token token = Read(input, index);
				if (token == null)
				{
					output.Clear();
					return output;
				}

				output.Add(token);

				index += token.ToString().Length;

				if (index == input.Length) break;

				if (index > input.Length)
				{
					output.Clear();
					return output;
				}
			}

			return output;
		}

		Token Read(String input, Int32 index)
		{
			if (input == null) throw new ArgumentNullException("input");
			if (index < 0 || index >= input.Length) throw new ArgumentOutOfRangeException("index");

			//Read Quoted String
			if (input[index] == '"')
			{
				Int32 endquoteindex = input.IndexOf('"', index + 1);
				if (endquoteindex != -1)
				{
					String text = input.Substring(index, endquoteindex + 1 - index);
					return new Token(text, m_textdata);
				}
				else
				{
					String text = input.Substring(index, 1);
					return new Token(text, m_unknowndata);
				}
			}

			//Read Identifier
			if (Char.IsLetter(input[index]) == true)
			{
				Int32 length = 1;
				while (index + length < input.Length && (Char.IsLetterOrDigit(input[index + length]) == true || input[index + length] == '.')) ++length;

				return MakeToken(input, index, length);
			}

			//Read Number
			if (Char.IsNumber(input[index]) == true || input[index] == '.')
			{
				Int32 length = 1;
				while (index + length < input.Length && (Char.IsNumber(input[index + length]) == true || input[index + length] == '.')) ++length;

				String text = input.Substring(index, length);

				if (m_intdata.Match(text) == true) return new Token(text, m_intdata);
				if (m_floatdata.Match(text) == true) return new Token(text, m_floatdata);
				return new Token(text, m_unknowndata);
			}

			//Read Mathematical Symbol
			if (Char.IsLetterOrDigit(input[index]) == false)
			{
				if (index + 1 < input.Length && (Char.IsWhiteSpace(input[index + 1]) == false && Char.IsLetterOrDigit(input[index + 1]) == false))
				{
					Token token = MakeToken(input, index, 2);
					if (token != null && token.Data != m_unknowndata) return token;
				}

				return MakeToken(input, index, 1);
			}

			return null;
		}

		Token MakeToken(String input, Int32 startindex, Int32 length)
		{
			if (input == null) throw new ArgumentNullException("input");

			String text = input.Substring(startindex, length);

			TokenData data;
			if (m_tokenmap.TryGetValue(text, out data) == true) return new Token(text, data);

			return new Token(text, m_unknowndata);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<String, TokenData> m_tokenmap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TokenData m_intdata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TokenData m_floatdata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TokenData m_textdata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TokenData m_unknowndata;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Regex m_hackregex;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly MatchEvaluator m_hackmatchevaluator;

		#endregion
	}
}