using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace csj
{
	class BuildException : Exception
	{
		public BuildException(string message)
			: base(message)
		{
		}
	}

	class NamespaceInfo
	{
		public NamespaceInfo(string name)
		{
			Name = name;
		}

		public string Name;
		public StringBuilder Code = new StringBuilder();

		public NamespaceInfo Parent;
		public Dictionary<string, NamespaceInfo> Children = new Dictionary<string,NamespaceInfo>();

		string _currentSection;
		public void StartSection(string name)
		{
			if (_currentSection == name)
				return;

			if (_currentSection!=null)
				Code.Append("\r\n");
			_currentSection = name;
		}

		public string NestedName
		{
			get
			{
				int dotPos = Name.LastIndexOf('.');
				if (dotPos<0)
					return Name;
				else
					return Name.Substring(dotPos + 1);
			}
		}

		public int SkipLeadingWhitespace(Parser p)
		{
			// Remember start of line
			var startOfLine = p.pos;

			int leadingSpaces = 0;
			while (true)
			{
				if (p[0] == ' ')
				{
					leadingSpaces++;
					p.pos++;
				}
				else if (p[0] == '\t')
				{
					leadingSpaces += 4;
					leadingSpaces -= leadingSpaces % 4;
					p.pos++;
				}
				else
					break;
			}

			// Ignore preprocessor directives and blank lines
			if (p[0] == '\r' || p[0] == '\n')
			{
				return -1;
			}

			return leadingSpaces / 4;
		}

		public int FindCommonLeadingTabs(Parser p)
		{
			int commonTabs = -1;
			while (p[0] != '\0')
			{
				int leadingTabs = SkipLeadingWhitespace(p);
				if (leadingTabs >= 0)
				{
					// Don't include preprocessor directives in leading calcs
					if (p[0] != '#')
					{
						if (commonTabs == -1)
							commonTabs = leadingTabs;
						else if (leadingTabs < commonTabs)
							commonTabs = leadingTabs;
					}
				}

				// Next line
				p.SkipToEOL();
				p.SkipEOL();
			}

			if (commonTabs == -1)
				commonTabs = 0;

			return commonTabs;
		}
		

		public void Write(TextWriter w, string indent)
		{
			w.WriteLine("{0}namespace {1}", indent, NestedName);
			w.WriteLine("{0}{{", indent);

			// Work out the common prefix of all lines
			var p = new Parser(Code.ToString());
			int commonTabs = FindCommonLeadingTabs(p);

			// Rewind
			p.pos = 0;

			while (p[0] != '\0')
			{
				int lineStart = p.pos;

				int leadingTabs = SkipLeadingWhitespace(p);
				
				if (leadingTabs >= commonTabs)
				{
					w.Write(indent);
					w.Write(new string('\t', leadingTabs - commonTabs + 1));
				}

				int lineSignificantStart = p.pos;

				// Find the end of the line
				p.SkipToEOL();
				p.SkipEOL();

				w.Write(p.code.Substring(lineSignificantStart, p.pos-lineSignificantStart));
			}

			//w.WriteLine(Code.ToString());

			// Write nested classes
			foreach (var n in Children.Values)
			{
				w.WriteLine();
				n.Write(w, indent + "\t");
			}

			w.WriteLine("{0}}}", indent);
		}
	}

	class Program
	{
		CommandLine _cl;

		string _leadingComments;
		List<string> _usingClauses = new List<string>();
		Dictionary<string, NamespaceInfo> _namespaces = new Dictionary<string, NamespaceInfo>();
		Parser p;
		string _currentFile;


		NamespaceInfo GetOrCreateNamespace(string name)
		{
			NamespaceInfo ns;
			if (!_namespaces.TryGetValue(name, out ns))
			{
				ns = new NamespaceInfo(name);
				AddNamespace(ns);
			}

			return ns;
		}

		void AddNamespace(NamespaceInfo ns)
		{
			// Find the parent namespace
			int dotPos = ns.Name.LastIndexOf('.');
			if (dotPos > 0)
			{
				ns.Parent = GetOrCreateNamespace(ns.Name.Substring(0, dotPos));

				ns.Parent.Children.Add(ns.NestedName, ns);
			}

			_namespaces.Add(ns.Name, ns);
		}

		public string ExtractLeadingComments()
		{
			p.SkipWhitespaceAndComments();

			if (p.pos == 0)
				return null;

			return p.code.Substring(0, p.pos);
		}

		void ParseNamespace(string outerNamespace)
		{
			p.SkipWhitespaceAndComments();

			// Parse the name of the namespace
			string name = outerNamespace;
			while (true)
			{
				if (!string.IsNullOrEmpty(name))
					name += ".";

				name += p.SkipIdentifier();

				if (p[0] == '.')
				{
					p.pos++;
					continue;
				}

				break;
			}

			// Find/create the namespace info
			NamespaceInfo ns = null;
			if (!_namespaces.TryGetValue(name, out ns))
			{
				ns = new NamespaceInfo(name);
				AddNamespace(ns);
			}

			// Skip the opening brace
			p.SkipWhitespaceAndComments();
			p.Skip("{");
			p.SkipLineSpace();
			p.SkipEOL();

			int startPos = p.pos;
			int braceDepth = 0;
			while (true)
			{
				int currentPos = p.pos;

				p.SkipWhitespaceAndComments();

				if (p.SkipString())
					continue;

				// Nested whitespace?
				if (p.SkipMatchOptional("namespace"))
				{
					if (braceDepth == 0)
						throw new BuildException("Unexpected 'namespace' keyword inside bracing");

					ns.StartSection(_currentFile);
					ns.Code.Append(p.code.Substring(startPos, currentPos-startPos)); 
					ParseNamespace(name);
					startPos = p.pos;
					continue;
				}


				// Opening brace?
				if (p[0] == '{')
				{
					braceDepth++;
					p.pos++;
					continue;
				}

				// Closing brace?
				if (p[0] == '}')
				{
					if (braceDepth == 0)
						break;

					braceDepth--;
					p.pos++;
					continue;
				}

				if (p[0] == '\0')
					throw new BuildException("Unterminated namespace block");

				// Anything else just skip it
				p.pos++;
			}

			ns.StartSection(_currentFile);
			ns.Code.Append(p.code.Substring(startPos, p.pos - startPos)); 

			p.Skip("}");
			p.SkipWhitespaceAndComments();
		}

		// Extract the using clauses and namespaces
		public void SplitCode()
		{
			while (true)
			{
				// Skip leading whitespace
				p.SkipWhitespaceAndComments();

				if (p.DoesMatch("using"))
				{
					int startPos = p.pos;
					while (p[0] != ';')
					{
						p.pos++;
						if (p[0] == '\0')
							throw new BuildException("Unterminated using clause");
					}

					p.pos++;
					_usingClauses.Add(p.code.Substring(startPos, p.pos - startPos));

					continue;
				}
					
				if (p.SkipMatchOptional("namespace"))
				{
					ParseNamespace("");
					continue;
				}

				// EOF?
				if (p[0] == '\0')
					return;

				throw new BuildException("Unrecognized content outside namespace block");
			}
		}


		int Run(string[] args)
		{
			_cl = new CommandLine();

			try
			{
				// Process arguments, quit if handled internally
				if (!_cl.ProcessArgs(args.ToList()))
					return 0;

				// Process files
				bool firstFile = true;
				foreach (var f in _cl.BuildFileList())
				{
					Console.WriteLine("Processing {0}...", f);

					// Load the file
					p = new Parser(System.IO.File.ReadAllText(f));
					_currentFile = f;

					// The first file is used for leading comments
					if (firstFile)
					{
						_leadingComments = ExtractLeadingComments();
						firstFile = false;
					}

					// Parse namespaces
					SplitCode();
				}

				// Write the output file
				using (var output = new StreamWriter(_cl.OutputFileName))
				{
					// Leading comments
					if (!String.IsNullOrEmpty(_leadingComments))
						output.WriteLine(_leadingComments);

					output.WriteLine("");
					output.WriteLine("// This file was built by merging separate C# source files into one.");
					output.WriteLine("// DO NOT EDIT THIS FILE - go back to the originals");
					output.WriteLine("");

					// Using Clauses
					foreach (var c in _usingClauses.Distinct().OrderBy(x=>x.TrimEnd(';', ' ', '\t')))
						output.WriteLine(c);

					output.WriteLine();

					// Namespaces
					foreach (var ns in _namespaces.Values)
					{
						if (ns.Parent == null)
							ns.Write(output, "");
					}
				}
			}
			catch (BuildException e)
			{
				_cl.ShowLogo();
				Console.WriteLine(e.Message);
				System.Environment.ExitCode = 7;
			}
			catch (CommandLine.Exception e)
			{
				_cl.ShowLogo();
				Console.WriteLine(e.Message);
				return 9;
			}
			catch (System.IO.IOException e)
			{
				_cl.ShowLogo();
				Console.WriteLine("File error - {0}", e.Message);
				System.Environment.ExitCode = 11;
			}
			return 0;
		}

		static int Main(string[] args)
		{
			return new Program().Run(args);
		}
	}
}
