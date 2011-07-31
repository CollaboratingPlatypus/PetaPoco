using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.IO;
using System.Diagnostics;

namespace PetaTest
{
	// Use to mark a class as a testfixture 
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class TestFixtureAttribute : TestBaseAttribute
	{
		public TestFixtureAttribute(params object[] args) : base(args) { }
	}

	// Use to mark a method as a test
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class TestAttribute : TestBaseAttribute
	{
		public TestAttribute(params object[] args) : base(args) { }
	}

	// Base class for Test and TestFixture attributes
	public abstract class TestBaseAttribute : Attribute
	{
		public TestBaseAttribute(params object[] Arguments)
		{
			this.Arguments = Arguments;
		}

		public object[] Arguments { get; private set; }
		public string Source { get; set; }
		public bool Active { get; set; }

		public virtual IEnumerable<object[]> GetArguments(Type owningType)
		{
			if (Source != null)
			{
				var iter_method = owningType.GetMethod(Source, BindingFlags.Static | BindingFlags.Public);
				return (IEnumerable<object[]>)iter_method.Invoke(null, null);
			}
			else
			{
				return new object[][] { Arguments };
			}
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class SetupAttribute : SetupTeardownAttributeBase
	{
		public SetupAttribute() : base(true, false) { }
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class TearDownAttribute : SetupTeardownAttributeBase
	{
		public TearDownAttribute() : base(false, false) { }
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class TestFixtureSetupAttribute : SetupTeardownAttributeBase
	{
		public TestFixtureSetupAttribute() : base(true, true) { }
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class TestFixtureTearDownAttribute : SetupTeardownAttributeBase
	{
		public TestFixtureTearDownAttribute() : base(false, true) { }
	}

	// Introducing "The Asserts" - all pretty self explanatory
	[SkipInStackTrace]
	public static partial class Assert
	{
		public static void Throw(bool Condition, Func<string> message)
		{
			if (!Condition)
				throw new AssertionException(message());
		}
		public static void IsTrue(bool test)
		{
			Throw(test, () => "Expression is not true");
		}

		public static void IsFalse(bool test)
		{
			Throw(!test, () => "Expression is not false");
		}

		public static void AreSame(object a, object b)
		{
			Throw(object.ReferenceEquals(a, b), () => "Object references are not the same");
		}

		public static void AreNotSame(object a, object b)
		{
			Throw(!object.ReferenceEquals(a, b), () => "Object references are the same");
		}

		public static void AreEqual(object a, object b)
		{
			Throw(object.Equals(a, b), () => string.Format("Objects are not equal\n  lhs: {0}\n  rhs: {1}", Utils.FormatValue(a), Utils.FormatValue(b)));
		}

		public static void AreNotEqual(object a, object b)
		{
			Throw(!object.Equals(a, b), () => string.Format("Objects are equal\n  lhs: {0}\n  rhs: {1}", Utils.FormatValue(a), Utils.FormatValue(b)));
		}

		public static void AreEqual(string a, string b)
		{
			Throw(string.Compare(a, b) == 0, () =>
			{
				var offset = Utils.CountCommonPrefix(a, b);
				var xa = Utils.FormatValue(Utils.GetStringExtract(a, offset));
				var xb = Utils.FormatValue(Utils.GetStringExtract(b, offset));
				return string.Format("Strings are not equal at offset {0}\n  lhs: {1}\n  rhs: {2}\n{3}^", offset, xa, xb, new string(' ', Utils.CountCommonPrefix(xa, xb) + 7));
			});
		}

		public static void IsEmpty(string val)
		{
			Throw(val != null && val.Length == 0, () => string.Format("String is not empty: {0}", Utils.FormatValue(val)));
		}

		public static void IsNotEmpty(string val)
		{
			Throw(val != null && val.Length != 0, () => "String is empty");
		}

		public static void IsEmpty(System.Collections.IEnumerable collection)
		{
			Throw(collection.Cast<object>().Count() == 0, () => string.Format("Collection is not empty\nItems: {0}", Utils.FormatValue(collection)));
		}

		public static void IsNotEmpty(System.Collections.IEnumerable collection)
		{
			Throw(collection.Cast<object>().Count() != 0, () => "Collection is empty");
		}

		public static void Contains(System.Collections.IEnumerable collection, object item)
		{
			Throw(collection.Cast<object>().Contains(item), () => string.Format("Collection doesn't contain {0}\nItems: {1}", Utils.FormatValue(item), Utils.FormatValue(collection)));
		}

		public static void DoesNotContain(System.Collections.IEnumerable collection, object item)
		{
			Throw(!collection.Cast<object>().Contains(item), () => string.Format("Collection does contain {0}", Utils.FormatValue(item)));
		}

		public static void IsNull(object val)
		{
			Throw(val == null, () => string.Format("Object reference is not null - {0}", Utils.FormatValue(val)));
		}
		public static void IsNotNull(object val)
		{
			Throw(val != null, () => "Object reference is null");
		}
		public static void Compare(object a, object b, Func<int, bool> Check, string comparison)
		{
			Throw(Check((a as IComparable).CompareTo(b)), () => string.Format("Comparison not true: {0} {1} {2}", Utils.FormatValue(a), comparison, Utils.FormatValue(b)));
		}

		public static void Greater(object a, object b)
		{
			Compare(a, b, r => r > 0, ">");
		}

		public static void GreaterOrEqual(object a, object b)
		{
			Compare(a, b, r => r >= 0, ">");
		}

		public static void Less(object a, object b)
		{
			Compare(a, b, r => r < 0, ">");
		}

		public static void LessOrEqual(object a, object b)
		{
			Compare(a, b, r => r <= 0, ">");
		}

		public static void IsInstanceOf(Type t, object o)
		{
			IsNotNull(o); Throw(o.GetType() == t, () => string.Format("Object type mismatch, expected {0} found {1}", t.FullName, o.GetType().FullName));
		}

		public static void IsNotInstanceOf(Type t, object o)
		{
			IsNotNull(o); Throw(o.GetType() != t, () => string.Format("Object type mismatch, should not be {0}", t.FullName));
		}

		public static void IsAssignableFrom(Type t, object o)
		{
			IsNotNull(o); Throw(t.IsAssignableFrom(o.GetType()), () => string.Format("Object type mismatch, expected a type assignable from {0} found {1}", t.FullName, o.GetType().FullName));
		}

		public static void IsNotAssignableFrom(Type t, object o)
		{
			IsNotNull(o); Throw(!t.IsAssignableFrom(o.GetType()), () => string.Format("Object type mismatch, didn't expect a type assignable from {0} found {1}", t.FullName, o.GetType().FullName));
		}

		public static void IsInstanceOf<T>(object o)
		{
			IsInstanceOf(typeof(T), o);
		}

		public static void IsNotInstanceOf<T>(object o)
		{
			IsNotInstanceOf(typeof(T), o);
		}

		public static void IsAssignableFrom<T>(object o)
		{
			IsAssignableFrom(typeof(T), o);
		}

		public static void IsNotAssignableFrom<T>(object o)
		{
			IsNotAssignableFrom(typeof(T), o);
		}

		public static void Throws(Type t, Action code)
		{
			try
			{
				code();
			}
			catch (Exception x)
			{
				Throw(t.IsAssignableFrom(x.GetType()), () => string.Format("Wrong exception type caught, expected {0} received {1}", t.FullName, Utils.FormatValue(x)));
				return;
			}
			throw new AssertionException(string.Format("Failed to throw exception of type {0}", t.FullName));
		}

		public static void Throws<TX>(Action code)
		{
			Throws(typeof(TX), code);
		}

		public static void DoesNotThrow(Action code)
		{
			try
			{
				code();
			}
			catch (Exception x)
			{
				Throw(false, () => string.Format("Unexpected exception {0}", Utils.FormatValue(x)));
			}
		}

		public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> code)
		{
			int index = 0;
			foreach (var i in source)
			{
				try
				{
					code(i);
				}
				catch (Exception x)
				{
					throw new AssertionException(string.Format("Collection assertion failed at item {0}\nCollection: {1}\nInner Exception: {2}", index, Utils.FormatValue(source), x.Message));
				}
				index++;
			}
		}
	}

	// Runner - runs a set of tests
	public class Runner
	{
		public Runner()
		{
			Output = new HtmlResultsWriter();
		}

		public ResultsWriter Output
		{
			get;
			set;
		}

		// Run all test fixtures in the calling assembly - unless one or more
		// marked as active in which case only those will be run
		public int Run(string[] args)
		{
			_statsStack.Clear();
			_statsStack.Push(new Stats());
			var old = Console.Out;
			Console.SetOut(Output);

			RunInternal(Assembly.GetCallingAssembly(), null, null, false);

			Console.SetOut(old);
			Output.Complete(_statsStack.Pop());
			return 0;
		}

		// Helper to create instances of test fixtures
		private object CreateInstance(Type t, object[] args)
		{
			try
			{
				return Activator.CreateInstance(t, args);
			}
			catch (Exception x)
			{
				Output.WriteException(x);
				Stats.Errors++;
				return null;
			}
		}

		// Internally called to recursively run tests in an assembly, testfixture, test method etc...
		private void RunInternal(object scope, object instance, object[] arguments, bool RunAll)
		{
			// Assembly?
			var a = scope as Assembly;
			if (a != null)
			{
				StartTest(a, null);
				RunAll = RunAll || !a.HasActive();
				foreach (var type in a.GetTypes().Where(i => i.IsTestFixture() && (RunAll || i.HasActive())))
					RunInternal(type, null, null, RunAll);
				EndTest();
			}

			// Test Fixture class
			var t = scope as Type;
			if (t != null)
			{
				if (arguments == null)
				{
					bool runAllTestFixturesInstances = RunAll || !t.IsActive();
					bool runAllTestMethods = RunAll || !t.HasActiveMethods();
					foreach (TestFixtureAttribute tfa in t.GetCustomAttributes(typeof(TestFixtureAttribute), false).Where(x => runAllTestFixturesInstances || ((TestFixtureAttribute)x).Active))
						foreach (var args in tfa.GetArguments(t))
							RunInternal(t, null, args, runAllTestMethods);
				}
				else
				{
					StartTest(t, arguments);
					var inst = CreateInstance(t, arguments);
					if (inst != null)
						RunInternal(null, inst, null, RunAll);
					EndTest();
				}
			}

			// Test Fixture instance
			if (instance != null && instance.GetType().IsTestFixture())
			{
				var tf = instance;
				if (scope == null)
				{
					if (!RunSetupTeardown(instance, true, true))
						return;

					foreach (var m in tf.GetType().GetMethods().Where(x => RunAll || x.IsActive()))
						RunInternal(m, instance, null, RunAll);

					RunSetupTeardown(instance, false, true);
				}

				var method = scope as MethodInfo;
				if (method != null)
				{
					if (arguments == null)
					{
						foreach (TestAttribute i in method.GetCustomAttributes(typeof(TestAttribute), false).Where(x => RunAll || ((TestAttribute)x).Active))
							foreach (var args in i.GetArguments(method.DeclaringType))
							{
								if (args.Length != method.GetParameters().Length)
								{
									Output.WriteWarning("{0} provided in an incorrect number of arguments (expected {1} but found {2}) - skipped", i.GetType().FullName, method.GetParameters().Length, args.Length);
									Stats.Warnings++;
									continue;
								}

								RunInternal(method, instance, args, RunAll);
							}
					}
					else
					{
						RunTest(method, tf, arguments);
					}
				}
			}

		}

		// Run a single test
		public void RunTest(MethodInfo Target, object instance, object[] Params)
		{

			StartTest(Target, Params);
			var sw = new Stopwatch();
			try
			{
				if (!RunSetupTeardown(instance, true, false))
				{
					EndTest();
					return;
				}
				sw.Start();
				Target.Invoke(instance, Params);
				Stats.Elapsed = sw.ElapsedMilliseconds;
				Stats.Passed++;
			}
			catch (Exception x)
			{
				Stats.Elapsed = sw.ElapsedMilliseconds;
				var invoc = x as TargetInvocationException;
				if (invoc != null)
					x = invoc.InnerException;
				Output.WriteException(x);
				Stats.Errors++;
			}
			RunSetupTeardown(instance, false, false);
			EndTest();
		}

		[SkipInStackTrace]
		public bool RunSetupTeardown(object instance, bool setup, bool fixture)
		{
			try
			{
				foreach (var m in instance.GetType().GetMethods().Where(x => x.GetCustomAttributes(typeof(SetupTeardownAttributeBase), false).Cast<SetupTeardownAttributeBase>().Any((SetupTeardownAttributeBase y) => y.ForSetup == setup && y.ForFixture == fixture)))
				{
					m.Invoke(instance, null);
				}
				return true;
			}
			catch (Exception x)
			{
				var invoc = x as TargetInvocationException;
				if (invoc != null)
					x = invoc.InnerException;
				Output.WriteException(x);
				Stats.Errors++;
				return false;
			}
		}


		private void StartTest(object Target, object[] Params)
		{
			var stats = new Stats() { Target = Target };
			_statsStack.Push(stats);
			Output.StartTest(Target, Params);
		}

		private void EndTest()
		{
			var old = Stats;
			_statsStack.Pop();
			Stats.Add(old);
			Output.EndTest(old);
		}

		private Stack<Stats> _statsStack = new Stack<Stats>();
		public Stats Stats { get { return _statsStack.Peek(); } }
	}

	public class Stats
	{
		public object Target { get; set; }
		public int Errors { get; set; }
		public int Warnings { get; set; }
		public int Passed { get; set; }
		public long Elapsed { get; set; }

		public void Add(Stats other)
		{
			Errors += other.Errors;
			Warnings += other.Warnings;
			Passed += other.Passed;
			Elapsed += other.Elapsed;
		}
	}

	// The exception thrown when an assertion fails
	public class AssertionException : Exception
	{
		public AssertionException(string message) : base(message) { }
	}

	// Used to mark utility functions that throw assertion exceptions so the stack trace can be unwound to the actual place the assertion originates
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class SkipInStackTraceAttribute : Attribute
	{
	}

	// Base class for setup/teardown attributes
	public class SetupTeardownAttributeBase : Attribute
	{
		public SetupTeardownAttributeBase(bool forSetup, bool forFixture)
		{
			this.ForSetup = forSetup;
			this.ForFixture = forFixture;
		}
		public bool ForSetup { get; set; }
		public bool ForFixture { get; set; }
	}

	// A bunch of utility functions and extension methods
	public static class Utils
	{
		public static IEnumerable<StackFrame> SimplifyStackTrace(StackTrace st)
		{
			foreach (var f in st.GetFrames())
			{
				if (f.GetMethod().GetCustomAttributes(typeof(SkipInStackTraceAttribute), false).Length != 0 ||
					f.GetMethod().DeclaringType.GetCustomAttributes(typeof(SkipInStackTraceAttribute), false).Length != 0 ||
					f.GetFileName() == null)
					continue;

				if (f.GetMethod().IsSpecialName | f.GetMethod().Name.StartsWith("<"))
					break;

				yield return f;
			}
		}

		public static IEnumerable<Tuple<int, string>> ExtractLinesFromTextFile(string file, int line, int extra = 2)
		{
			try
			{
				if (line <= extra)
					line = extra + 1;

				return System.IO.File.ReadAllLines(file).Skip(line - extra - 1).Take(extra * 2 + 1).Select((l, i) => new Tuple<int, string>(i + line - extra, l));
			}
			catch (Exception)
			{
				return new Tuple<int, string>[] { };
			}
		}

		// Format any value for diagnostic display
		public static string FormatValue(object value)
		{
			if (value == null)
				return "null";

			var str = value as string;
			if (str != null)
			{
				str = str.Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\0", "\\0");
				return string.Format("\"{0}\"", str);
			}

			if (value.GetType() == typeof(int) || value.GetType() == typeof(long) || value.GetType() == typeof(bool))
				return value.ToString();

			var d = value as System.Collections.IDictionary;
			if (d != null)
				return string.Format("{{{0}}}", string.Join(", ", d.AsDictionaryEntries().Select(de => string.Format("{{ {0}, {1} }}", FormatValue(de.Key), FormatValue(de.Value)))));

			var e = value as System.Collections.IEnumerable;
			if (e != null)
				return string.Format("[{0}]", string.Join(", ", e.Cast<object>().Select(v => FormatValue(v))));

			var x = value as Exception;
			if (x != null)
				return string.Format("[{0}] {1}", value.GetType().FullName, x.Message);

			return string.Format("[{0}] {1}", value.GetType().FullName, value.ToString());
		}

		public static IEnumerable<System.Collections.DictionaryEntry> AsDictionaryEntries(this System.Collections.IDictionary dictionary)
		{
			foreach (var de in dictionary)
				yield return (System.Collections.DictionaryEntry)de;
		}

		public static string FormatArguments(object[] args)
		{
			return string.Format("({0})", args == null ? "" : string.Join(", ", args.Select(v => FormatValue(v))));
		}

		// Format the name of a test target
		public static string FormatTarget(object o)
		{
			var mb = o as MethodBase;
			if (mb != null)
				return "test " + mb.Name;
			var t = o as Type;
			if (t != null && t.IsClass)
				return "testfixture " + t.Name;
			var a = o as Assembly;
			if (a != null)
				return "assembly " + a.FullName;
			return null;
		}

		public static int CountCommonPrefix(string a, string b)
		{
			int i = 0;
			while (i < Math.Min(a.Length, b.Length) && a[i] == b[i])
				i++;
			return i;
		}

		public static string GetStringExtract(string str, int offset)
		{
			if (offset > 15)
				str = "..." + str.Substring(offset - 10);
			if (str.Length > 30)
				str = str.Substring(0, 20) + "...";
			return str;
		}

		public static bool IsTestFixture(this Type t)
		{
			return t.IsClass && !t.IsAbstract && t.GetCustomAttributes(typeof(TestFixtureAttribute), false).Any();
		}

		public static bool IsTestMethod(this MethodInfo mi)
		{
			return mi.GetCustomAttributes(typeof(TestAttribute), false).Any();
		}

		public static bool IsActive(this ICustomAttributeProvider p)
		{
			return p.GetCustomAttributes(typeof(TestBaseAttribute), false).Any(a => ((TestBaseAttribute)a).Active);
		}

		public static bool HasActiveMethods(this Type t)
		{
			return t.GetMethods().Any(m => m.IsActive());
		}

		public static bool HasActive(this Type t)
		{
			return t.IsActive() || t.HasActiveMethods();
		}

		public static bool HasActive(this Assembly a)
		{
			return a.GetTypes().Any(t => t.HasActive());
		}
	}

	// Base class for result writers
	public abstract class ResultsWriter : TextWriter
	{
		public abstract void StartTest(object Target, object[] Arguments);
		public abstract void EndTest(Stats stats);
		public virtual void Complete(Stats stats)
		{
		}

		public virtual void WriteWarning(string str, params object[] args)
		{
			WriteLine(str, args);
		}

		public virtual void WriteError(string str, params object[] args)
		{
			WriteLine(str, args);
		}

		public abstract void WriteException(Exception x);
	}

	// Plain text results writer (aka console output)
	public class PlainTextResultsWriter : ResultsWriter
	{
		TextWriter target;
		public PlainTextResultsWriter(TextWriter target = null)
		{
			this.target = target == null ? Console.Out : target;
		}

		public override void StartTest(object Target, object[] Arguments)
		{
			WriteIndented(string.Format("{0}{1}\n", Utils.FormatTarget(Target), Utils.FormatArguments(Arguments)));
			_indentDepth++;
		}

		public override void EndTest(Stats stats)
		{
			_indentDepth--;
		}

		public override void Complete(Stats stats)
		{
			bool Success = stats.Errors == 0 && stats.Warnings == 0;
			var delim = new string(Success ? '-' : '*', 40);
			if (Success)
				target.WriteLine("\n{0}\nAll {1} tests passed!\n{0}\n", delim, stats.Passed);
			else
				target.WriteLine("\n{0}\n{1} Errors, {2} Warnings, {3} passed\n{0}\n", delim, stats.Errors, stats.Warnings, stats.Passed);
		}

		public override void Write(char value)
		{
			Write(value.ToString());
		}

		public override void Write(char[] buffer, int index, int count)
		{
			Write(new String(buffer, index, count));
		}

		public override void Write(string str)
		{
			WriteIndented(str);
		}

		public void WriteIndented(string str)
		{
			string indent = new string(' ', _indentDepth * 2);
			if (_indentPending)
				target.Write(indent);

			_indentPending = str.EndsWith("\n");
			if (_indentPending)
				str = str.Substring(0, str.Length - 1);

			str = str.Replace("\n", "\n" + indent);

			target.Write(str);

			if (_indentPending)
				target.Write("\n");
		}

		public override Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}

		public override void WriteException(Exception x)
		{
			var assert = x as AssertionException;
			if (assert != null)
				WriteIndented(string.Format("\nAssertion failed - {0}\n\n", assert.Message));
			else
				WriteIndented(string.Format("\nException {0}: {1}\n\n", x.GetType().FullName, x.Message));

			StackFrame first = null;
			foreach (var f in Utils.SimplifyStackTrace(new StackTrace(x, true)))
			{
				if (first == null) first = f;
				WriteIndented(string.Format("  {0} - {1}({2})\n", f.GetMethod().Name, f.GetFileName(), f.GetFileLineNumber()));
			}

			if (first != null)
			{
				WriteIndented("\n");
				foreach (var l in Utils.ExtractLinesFromTextFile(first.GetFileName(), first.GetFileLineNumber()))
					WriteIndented(string.Format("  {0:00000}:{1}{2}\n", l.Item1, l.Item1 == first.GetFileLineNumber() ? "->" : "  ", l.Item2));
			}

			WriteIndented("\n");
		}

		bool _indentPending = true;
		int _indentDepth = 0;
	}

	// HTML Results Writer
	public class HtmlResultsWriter : PlainTextResultsWriter
	{
		public HtmlResultsWriter()
		{
			_tagHtml.Append(new TagBuilder("head").Append(StylesAndScript));

			var body = new TagBuilder("body");
			_tagHtml.Append(body);
			_current = body;
		}

		TagBuilder _tagHtml = new TagBuilder("html");
		TagBuilder _current;
		TagBuilder _pre;

		public override void StartTest(object Target, object[] Arguments)
		{
			// Work out title
			string title = "", css_class = "";
			var a = Target as Assembly;
			var t = Target as Type;
			var m = Target as MethodInfo;
			if (a != null)
			{
				title = "Test Results for " + a.ManifestModule.Name;
				css_class = "assembly";
			}
			else if (t != null)
			{
				title = string.Format("Test Fixture {0}{1}", t.FullName, Utils.FormatArguments(Arguments));
				css_class = "testfixture";
			}
			else if (m != null)
			{
				title = string.Format("{0}{1}", m.Name, Utils.FormatArguments(Arguments));
				css_class = "test";
			}

			// Create the test div
			var divTest = new TagBuilder("div");
			divTest.SetAttribute("class", css_class);
			divTest.Append(new TagBuilder("div").SetAttribute("class", "title")
				.Append(new TagBuilder("span").AddClass("indicator"))
				.Append(new TagBuilder("a").AddClass("toggle").SetAttribute("href", "#").EncodeAndAppend(title)));

			// Make room for assembly summary which we'll populate later
			if (a != null)
				divTest.Append(new TagBuilder("div").SetAttribute("class", "summary"));

			// Make the collapsable content div
			var divContent = new TagBuilder("div").SetAttribute("class", "content");
			divTest.Append(divContent);

			// Prepare to continue writing in the collapsable content div
			_current.Append(divTest);
			_current = divContent;
			_pre = null;

			base.StartTest(Target, Arguments);
		}

		public override void EndTest(Stats stats)
		{
			base.EndTest(stats);

			// If this was a test, and it passed, write something to indicate that
			if (stats.Target as MethodInfo != null && stats.Passed == 1)
			{
				WriteToTestOutput("Test passed!\n");
				_current.Parent.Find("div", "title", null).Append(string.Format(" <small>{0}ms</small>", stats.Elapsed));
			}

			// Pop the content div
			_current = _current.Parent;

			// Flag the test/testfixture/assembly as pass/fail and collapse passed tests
			if (stats.Errors == 0)
				_current.AddClass("pass" + ((stats.Target as MethodInfo != null) ? " collapsed" : ""));
			else
				_current.AddClass("fail");

			// Write the summary
			var t = _current.Find("div", "summary", null);
			if (t != null)
			{
				t.Append(string.Format("Passed: {0} Failed: {1} Warnings: {2} Elapsed: {3}ms", stats.Passed, stats.Errors, stats.Warnings, stats.Elapsed));
			}

			// Pop stack
			_current = _current.Parent;
		}

		public override void Complete(Stats stats)
		{
			base.Complete(stats);

			base.Write("View Report?");
			var key = Console.ReadKey();
			base.WriteLine();
			if (key.KeyChar == 'Y' || key.KeyChar == 'y')
			{
				using (var output = new StreamWriter("report.html"))
				{
					_tagHtml.Render(output);
				}
				System.Diagnostics.Process.Start("report.html");
			}
		}

		public void WriteToTestOutput(string str)
		{
			if (_pre == null)
			{
				_pre = new TagBuilder("pre");
				_current.Append(_pre);
			}

			_pre.EncodeAndAppend(str);
		}

		public override void Write(string str)
		{
			base.Write(str);
			WriteToTestOutput(str);
		}

		public override void WriteException(Exception x)
		{
			base.WriteException(x);

			// Split exception message into title and detail
			string title, detail = null;
			var assert = x as AssertionException;
			if (assert != null)
			{
				int cr = assert.Message.IndexOf('\n');
				if (cr > 0)
				{
					title = assert.Message.Substring(0, cr);
					detail = assert.Message.Substring(cr + 1);
				}
				else
					title = assert.Message;
				title = string.Format("Assertion failed - {0}", title);
			}
			else
			{
				title = string.Format("Exception {0}: {1}", x.GetType().FullName, x.Message);
			}


			var div = new TagBuilder("div").SetAttribute("class", "exception");
			div.Append(new TagBuilder("div").SetAttribute("class", "errormessage").EncodeAndAppend(title));

			if (detail != null)
			{
				div.Append(new TagBuilder("p").EncodeAndAppend("Detail"));
				div.Append(new TagBuilder("pre").EncodeAndAppend(detail));
			}

			div.Append(new TagBuilder("p").EncodeAndAppend("Stack Trace"));
			var st = new TagBuilder("pre").SetAttribute("class", "stacktrace");
			div.Append(st);

			StackFrame first = null;
			foreach (var f in Utils.SimplifyStackTrace(new StackTrace(x, true)))
			{
				if (first == null) first = f;
				st.EncodeAndAppend(string.Format("{0} - {1}({2})\n", f.GetMethod().Name, f.GetFileName(), f.GetFileLineNumber()));
			}

			if (first != null)
			{
				div.Append(new TagBuilder("p").EncodeAndAppend("Location"));

				var code = new TagBuilder("pre").SetAttribute("class", "code");
				div.Append(code);

				foreach (var l in Utils.ExtractLinesFromTextFile(first.GetFileName(), first.GetFileLineNumber()))
				{
					if (l.Item1 == first.GetFileLineNumber())
						code.Append("<span class=\"highlighted\">");
					code.EncodeAndAppend(string.Format("  {0:00000}:{1}{2}\n", l.Item1, l.Item1 == first.GetFileLineNumber() ? "->" : "  ", l.Item2));
					if (l.Item1 == first.GetFileLineNumber())
						code.Append("</span>");
				}
			}

			_pre = null;
			_current.Append(div);
		}

		private const string StylesAndScript = @"<style>
body { font-family:Arial; margin:20px; font-size:10pt}
div.assembly>div.title { font-size:20pt; margin-bottom:20px; border-bottom:1px solid silver; padding-bottom:20px; }
div.testfixture>div.title { font-size:16pt; }
div.testfixture { margin-bottom:20px; }
div.test { margin-left:20px; }
div.test div.title { font-size:12pt; }
div.errormessage { color:Red; padding-top:10px; }
div.pass>div.title { color:#808080; }
span.highlighted { color:Red; }
pre.code { background-color:#f0f0f0; }
div.summary { border:1px solid silver; padding:10px; margin-bottom:20px; text-align:center; }
div.fail div.summary { background-color:Red; color:White; }
div.pass div.summary { background-color:Lime; }
div.collapsed div.content { display:none; }
div.test div.content { border-left:2px solid #d0d0d0; padding-left:10px; margin-left:10px; }
a { text-decoration:none; color:#606060; }
a:hover { color:orange; }
div>div.title>span.indicator { display:inline-block; width:10px; height:10px; background-color:lime; border-radius:7px}
div.fail>div.title>span.indicator { display:inline-block; width:10px; height:10px; background-color:red; }
div.assembly>div.title>span.indicator { display:none; }
</style>
<script>
window.addEventListener(""load"", function () {
    var divs = document.getElementsByClassName(""toggle"");
    for (var i = 0; i < divs.length; i++) {
        divs[i].onclick = function () {
            var top = this.parentNode.parentNode;
            if (top.className.indexOf("" collapsed"") < 0)
                top.className += "" collapsed"";
            else
                top.className = top.className.replace("" collapsed"", """");
            return false;
        }
    }
});
</script>";
	}

	public class TagBuilder
	{
		public TagBuilder(string tagName)
		{
			Name = tagName;
		}

		public string Name { get; set; }
		public TagBuilder Parent { get; set; }
		public Dictionary<string, string> Attributes = new Dictionary<string, string>();

		public void Render(TextWriter w)
		{
			w.Write("<"); w.Write(Name);
			foreach (var a in Attributes)
			{
				w.Write(" "); w.Write(a.Key);
				if (a.Value != null)
				{
					w.Write("=\""); w.Write(Encode(a.Value)); w.Write("\"");
				}
			}

			w.Write(">");
			if (Name != "pre")
				w.Write("\n");
			if (_Content != null)
			{
				bool NeedNewLine = false;
				foreach (var i in _Content)
				{
					var t = i as TagBuilder;
					if (t != null)
					{
						if (NeedNewLine)
							w.Write("\n");
						t.Render(w);
						NeedNewLine = false;
					}

					var s = i as String;
					if (s != null)
					{
						w.Write(s);
						NeedNewLine = true;
					}
				}
			}
			if (Name != "pre")
				w.Write("\n");
			w.Write("</"); w.Write(Name); w.Write(">\n");
		}

		public TagBuilder Append(TagBuilder t)
		{
			if (_Content == null)
				_Content = new List<object>();
			t.Parent = this;
			_Content.Add(t);
			return this;
		}

		public TagBuilder Append(string s)
		{
			if (_Content == null)
				_Content = new List<object>();
			_Content.Add(s);
			return this;
		}

		public TagBuilder EncodeAndAppend(string s)
		{
			Append(Encode(s));
			return this;
		}

		public TagBuilder SetAttribute(string name, string value)
		{
			Attributes[name] = value;
			return this;
		}

		public TagBuilder AddClass(string className)
		{
			if (Attributes.ContainsKey("class"))
				Attributes["class"] = Attributes["class"] + " " + className;
			else
				Attributes["class"] = className;
			return this;
		}

		public TagBuilder Find(string tagName, string className, string id)
		{
			foreach (TagBuilder t in _Content.Where(x => x as TagBuilder != null))
			{
				if (tagName != null && t.Name != tagName)
					continue;
				if (className != null && t.Attributes["class"] != className)
					continue;
				if (id != null && t.Attributes["id"] != id)
					continue;
				return t;
			}
			return null;
		}

		public static string Encode(string str)
		{
			return str.Replace("&", "&amp;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("\'", "&#039;").Replace(">", "&gt;");
		}

		List<object> _Content;
	}

}
