using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace xnaMugen.Evaluation
{
	internal class Tokenizer
	{
		public Tokenizer()
		{
			m_intdata = new Tokenizing.IntData();
			m_floatdata = new Tokenizing.FloatData();
			m_textdata = new Tokenizing.TextData();
			m_unknowndata = new Tokenizing.UnknownData();
			m_tokenmap = BuildTokenMapping();
		}

		private Dictionary<string, TokenData> BuildTokenMapping()
		{
			var mapping = new Dictionary<string, TokenData>(StringComparer.OrdinalIgnoreCase);

			foreach (var field in typeof(Operator).GetFields())
			{
				var bomAttrib = (BinaryOperatorMappingAttribute)Attribute.GetCustomAttribute(field, typeof(BinaryOperatorMappingAttribute));
				if (bomAttrib != null)
				{
					var @operator = (Operator)field.GetValue(null);
					mapping.Add(bomAttrib.Value, new Tokenizing.BinaryOperatorData(@operator, bomAttrib.Value, bomAttrib.Name, bomAttrib.Precedence));
				}
				else
				{
					var fm_attr = (UnaryOperatorMappingAttribute)Attribute.GetCustomAttribute(field, typeof(UnaryOperatorMappingAttribute));
					if (fm_attr != null)
					{
						var @operator = (Operator)field.GetValue(null);
						mapping.Add(fm_attr.Value, new Tokenizing.UnaryOperatorData(@operator, fm_attr.Value, fm_attr.Name));
					}
				}
			}

			foreach (var field in typeof(Symbol).GetFields())
			{
				var attr = (TokenMappingAttribute)Attribute.GetCustomAttribute(field, typeof(TokenMappingAttribute));
				if (attr != null)
				{
					var symbol = (Symbol)field.GetValue(null);
					mapping.Add(attr.Value, new Tokenizing.SymbolData(symbol, attr.Value));
				}
			}

			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsAbstract == false || type.IsClass == false) continue;

				var cf_attr = (CustomFunctionAttribute)Attribute.GetCustomAttribute(type, typeof(CustomFunctionAttribute));
				if (cf_attr != null) mapping.Add(cf_attr.Value, new Tokenizing.CustomFunctionData(cf_attr.Value, cf_attr.Value, type));

				var sr_attr = (StateRedirectionAttribute)Attribute.GetCustomAttribute(type, typeof(StateRedirectionAttribute));
				if (sr_attr != null) mapping.Add(sr_attr.Value, new Tokenizing.StateRedirectionData(sr_attr.Value, sr_attr.Value, type));
			}

			return mapping;
		}

		public List<Token> Tokenize(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			var output = new List<Token>();
			var index = 0;

			while (true)
			{
				while (index < input.Length && char.IsWhiteSpace(input[index])) ++index;
				if (index >= input.Length) break;

				var token = Read(input, index);
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

		private Token Read(string input, int index)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));
			if (index < 0 || index >= input.Length) throw new ArgumentOutOfRangeException(nameof(index));

			//Read Quoted String
			if (input[index] == '"')
			{
				var endquoteindex = input.IndexOf('"', index + 1);
				if (endquoteindex != -1)
				{
					var text = input.Substring(index, endquoteindex + 1 - index);
					return new Token(text, m_textdata);
				}
				else
				{
					var text = input.Substring(index, 1);
					return new Token(text, m_unknowndata);
				}
			}

			//Read Identifier
			if (char.IsLetter(input[index]))
			{
				var length = 1;
				while (index + length < input.Length && (char.IsLetterOrDigit(input[index + length]) || input[index + length] == '.')) ++length;

				return MakeToken(input, index, length);
			}

			//Read Number
			if (char.IsNumber(input[index]) || input[index] == '.')
			{
				var length = 1;
				while (index + length < input.Length && (char.IsNumber(input[index + length]) || input[index + length] == '.')) ++length;

				var text = input.Substring(index, length);

				if (m_intdata.Match(text)) return new Token(text, m_intdata);
				if (m_floatdata.Match(text)) return new Token(text, m_floatdata);
				return new Token(text, m_unknowndata);
			}

			//Read Mathematical Symbol
			if (char.IsLetterOrDigit(input[index]) == false)
			{
				if (index + 1 < input.Length && char.IsWhiteSpace(input[index + 1]) == false && char.IsLetterOrDigit(input[index + 1]) == false)
				{
					var token = MakeToken(input, index, 2);
					if (token != null && token.Data != m_unknowndata) return token;
				}

				return MakeToken(input, index, 1);
			}

			return null;
		}

		private Token MakeToken(string input, int startindex, int length)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			var text = input.Substring(startindex, length);

			TokenData data;
			if (m_tokenmap.TryGetValue(text, out data)) return new Token(text, data);

			return new Token(text, m_unknowndata);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<string, TokenData> m_tokenmap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly TokenData m_intdata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly TokenData m_floatdata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly TokenData m_textdata;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly TokenData m_unknowndata;

		#endregion
	}
}