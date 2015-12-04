using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csj
{
	class Parser
	{
		public Parser(string code, int pos=0)
		{
			this.code = code + "\0";
			this.pos = pos;
		}

		public string code;
		public int pos;

		public string Remainder
		{
			get
			{
				return code.Substring(pos);
			}
		}

		public char this[int offset]
		{
			get
			{
				return code[pos + offset];
			}
		}


		public void SkipWhitespaceAndComments()
		{
			while (true)
			{
				// Skip whitespace
				while (char.IsWhiteSpace(code[pos]))
					pos++;

				// Skip single line comments
				if (code[pos] == '/' && code[pos + 1] == '/')
				{
					pos += 2;
					while (code[pos] != '\r' && code[pos] != '\n')
						pos++;
					continue;
				}

				// Skip block comments
				if (code[pos] == '/' && code[pos + 1] == '*')
				{
					pos += 2;
					while (!(code[pos] == '*' && code[pos + 1] == '/'))
					{
						pos++;
						if (code[pos] == '\0')
							throw new BuildException("Unterminated block comment found");
					}

					pos += 2;
					continue;
				}

				break;
			}
		}

		public bool IsIdentifierChar(char ch)
		{
			return char.IsLetterOrDigit(ch) || ch == '_';
		}

		public bool IsIdentifierLeadChar(char ch)
		{
			return char.IsLetter(ch) || ch == '_';
		}

		public void SkipLineSpace()
		{
			while (code[pos] == ' ' || code[pos] == '\t')
				pos++;
		}

		public void SkipToEOL()
		{
			while (code[pos] != '\r' && code[pos] != '\n')
				pos++;
		}

		public bool SkipEOL()
		{
			if (code[pos] == '\r' && code[pos + 1] == '\n')
				pos += 2;
			else if (code[pos] == '\r' && code[pos + 1] == '\n')
				pos += 2;
			else if (code[pos] == '\r')
				pos += 1;
			else if (code[pos] == '\n')
				pos += 1;
			else
				return false;

			return true;
		}

		public string SkipIdentifier()
		{
			if (!IsIdentifierLeadChar(code[pos]))
				throw new BuildException("Expected an identifier");

			int start = pos;
			pos++;

			while (IsIdentifierChar(code[pos]))
				pos++;

			return code.Substring(start, pos - start);
		}

		public bool DoesMatch(string what)
		{
			if (pos + what.Length > code.Length)
				return false;

			if (code.Substring(pos, what.Length) != what)
				return false;

			if (IsIdentifierChar(what[0]) && IsIdentifierChar(code[pos + what.Length]))
				return false;

			return true;
		}

		public void Skip(string what)
		{
			if (!DoesMatch(what))
				throw new BuildException(string.Format("Unexpected content, expected: {0}", what));

			pos += what.Length;
		}

		public bool SkipMatchOptional(string what)
		{
			if (DoesMatch(what))
			{
				pos += what.Length;
				return true;
			}
			return false;
		}

		public bool SkipString()
		{
			// Regular string or char
			if (code[pos] == '\"' || code[pos] == '\'')
			{
				char chTerminator = code[pos];

				// Skip opening quote
				pos++;

				// Skip content
				while (true)
				{
					if (code[pos] == '\\')
					{
						pos += 2;
					}
					else if (code[pos] == chTerminator)
					{
						break;
					}
					else if (code[pos] == '\r' || code[pos] == '\n')
					{
						throw new BuildException("Unterminated sting literal");
					}
					else
					{
						pos++;
					}
				}

				// Skip closing quote
				pos++;

				return true;
			}

			// Raw string
			if (code[pos] == '@' && code[pos + 1] == '\"')
			{
				// Opening 
				pos += 2;

				// Content
				while (true)
				{
					if (code[pos] == '\"' && code[pos + 1] == '\"')
						pos += 2;
					else if (code[pos] == '\"')
						break;
					else
						pos++;
				}

				// Closing
				pos++;

				return true;
			}

			return false;
		}


	}
}
