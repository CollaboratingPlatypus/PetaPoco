using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace csj
{
	class CommandLine
	{
		bool _logoShown;
		bool _noLogo;


		public CommandLine()
		{
		}

		public static List<string> ParseCommandLine(string args)
		{
			var newargs = new List<string>();

			var temp = new StringBuilder();

			int i = 0;
			while (i < args.Length)
			{
				if (char.IsWhiteSpace(args[i]))
				{
					i++;
					continue;
				}

				bool bInQuotes = false;
				temp.Length = 0;
				while (i < args.Length && (!char.IsWhiteSpace(args[i]) && !bInQuotes))
				{
					if (args[i] == '\"')
					{
						if (args[i + 1] == '\"')
						{
							temp.Append("\"");
							i++;
						}
						else
						{
							bInQuotes = !bInQuotes;
						}
					}
					else
					{
						temp.Append(args[i]);
					}

					i++;
				}

				if (temp.Length > 0)
				{
					newargs.Add(temp.ToString());
				}
			}

			return newargs;
		}

		public bool ProcessArgs(IEnumerable<string> args)
		{
			if (args == null)
				return true;

			// Parse args
			foreach (var a in args)
			{
				if (!ProcessArg(a))
					return false;
			}

			return true;
		}

		public bool ProcessResponseFile(string filename)
		{
			// Get the fully qualified response file name
			string strResponseFile = System.IO.Path.GetFullPath(filename);

			// Load and parse the response file
			var args = ParseCommandLine(System.IO.File.ReadAllText(strResponseFile));

			// Set the current directory
			string OldCurrentDir = System.IO.Directory.GetCurrentDirectory();
			System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(strResponseFile));

			// Load the file
			bool bRetv = ProcessArgs(args);

			// Restore current directory
			System.IO.Directory.SetCurrentDirectory(OldCurrentDir);

			return bRetv;
		}

		public bool ProcessArg(string arg)
		{
			if (arg == null)
				return true;

			// Response file
			if (arg.StartsWith("@"))
			{
				return ProcessResponseFile(arg.Substring(1));
			}


			// Args are in format [/-]<switchname>[:<value>];
			if (arg.StartsWith("/") || arg.StartsWith("-"))
			{
				string SwitchName = arg.Substring(1);
				string Value = null;

				int colonpos = SwitchName.IndexOf(':');
				if (colonpos >= 0)
				{
					// Split it
					Value = SwitchName.Substring(colonpos + 1);
					SwitchName = SwitchName.Substring(0, colonpos);
				}

				switch (SwitchName)
				{
					case "h":
					case "?":
						ShowLogo();
						ShowHelp();
						return false;

					case "v":
						ShowLogo();
						return false;

					case "nologo":
						_noLogo = true;
						break;

					case "r":
						_recursive = true;
						break;

					case "nr":
						_recursive = false;
						break;

					case "o":
						_outputFileName = System.IO.Path.GetFullPath(Value);
						_excludeFiles.Add(_outputFileName);
						break;

					case "x":
						_excludeFiles.AddRange(EnumerateFiles(Value));
						break;

					default:
						throw new Exception(string.Format("Unknown switch '{0}'", arg));

				}
			}
			else
			{
				_inputFiles.AddRange(EnumerateFiles(arg));
			}

			return true;
		}

		IEnumerable<string> EnumerateFiles(string spec)
		{
			// Is it a wildcard?
			if (spec.Contains("*") || spec.Contains("?"))
			{
				// Find slash/backslash
				var slashPos = spec.LastIndexOfAny(new char[] { '\\', '/' });

				string folder;
				string pattern;
				if (slashPos < 0)
				{
					folder = System.IO.Directory.GetCurrentDirectory();
					pattern = spec;
				}
				else
				{
					folder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), spec.Substring(0, slashPos));
					pattern = spec.Substring(slashPos + 1);
				}

				// Add matching files
				return System.IO.Directory.EnumerateFiles(folder, pattern,
					_recursive ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly);
			}
			else
			{
				// Add a single file
				return new string[] { System.IO.Path.GetFullPath(spec) };
			}
		}

		bool _recursive = true;
		string _outputFileName;
		List<string> _inputFiles = new List<string>();
		List<string> _excludeFiles = new List<string>();

		public IEnumerable<string> BuildFileList()
		{
			return _inputFiles.Where(x => !_excludeFiles.Contains(x, StringComparer.InvariantCultureIgnoreCase)).Distinct();
		}


		public string OutputFileName
		{
			get
			{
				return _outputFileName;
			}
		}

		public void ShowLogo()
		{
			if (_logoShown || _noLogo)
				return;
			_logoShown = true;

			System.Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine("C# Join Toolv{0}", v);
			Console.WriteLine("Copyright (C) 2012 Topten Software. All Rights Reserved.");
			Console.WriteLine("");
		}

		public static void ShowHelp()
		{
			Console.WriteLine("C# Join Tool");
			Console.WriteLine("");
			Console.WriteLine("usage: csj [OPTIONS] <files>");
			Console.WriteLine("");
			Console.WriteLine("-o:<filename>               The output file");
			Console.WriteLine("-x:<filename>               A file to ignore (wildcards supported)");
			Console.WriteLine("-r                          Recursively search sub-folders for subsequent wildcards (default)");
			Console.WriteLine("-nr                         Don't recursively search sub-folders for subsequent wildcards");
			Console.WriteLine(" <files>                    The set of C# (.cs) files to join (wildcards supported)");
			Console.WriteLine("");
			Console.WriteLine("eg:");
			Console.WriteLine("> csj -o:PetaPoco.cs Database.cs -r *.cs -x:Properties\\AssemblyInfo.cs");
		}



		public class Exception : System.Exception
		{
			public Exception(string msg) :
				base(msg)
			{
			}
		}

	}
}
