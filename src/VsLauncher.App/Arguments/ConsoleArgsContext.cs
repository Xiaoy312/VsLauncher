using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Options;

namespace VsLauncher.Arguments
{
	public partial class ConsoleArgsContext
	{
		public static ConsoleArgsContext Parse(string[] args)
		{
			var context = new ConsoleArgsContext();

			if (args.Length > 0)
			{
				var unparsed = new Stack<string>(CreateOptionsFor(context).Parse(args) ?? Enumerable.Empty<string>());
				if (unparsed.TryPop(out var unparsed0))
				{
					context.Path = unparsed0;
					foreach (var item in unparsed)
					{
						context.Errors.Add(new ConsoleArgError(item, ConsoleArgErrorType.UnrecognizedArgument));
					}
				}
			}

			return context;
		}

		internal static OptionSet CreateOptionsFor(ConsoleArgsContext context)
		{
			// https://github.com/mono/mono/blob/main/mcs/class/Mono.Options/Mono.Options/Options.cs
			// '=' value is required, `:` value is optional
			return new OptionSet
			{
				{ "help|h", "Display this help screen.", TrySet(_ => context.IsHelp = true) },
				{ "verbose|v", "Use verbose log messages", TrySet(_ => context.IsVerbose = true) },
				{ "vs=", "Specify the version of Visual Studio used to launch.", TrySet(x => context.VsVersion = x) },
				{ "slnf:|f:", "Include .slnf in the launch options. Optionally, you can also specify a filter for both sln or slnf.", TrySet(x => { context.IncludeSlnf = true; context.Filter = x; }) },
			};

			Action<string> TrySet(Action<string> set)
			{
				return value =>
				{
					if (context != null)
					{
						try
						{
							set(value);
						}
						catch (Exception e)
						{
							context.Errors.Add(new ConsoleArgError(value, ConsoleArgErrorType.ValueAssignmentError, e));
						}
					}
				};
			}
			Action<string> TryParseAndSet<T>(Func<string, T> parse, Action<T> set)
			{
				return value =>
				{
					if (context != null)
					{
						var isParsing = true;
						try
						{
							var parsed = parse(value);
							isParsing = false;
							set(parsed);
						}
						catch (Exception e)
						{
							context.Errors.Add(new ConsoleArgError(
								value,
								isParsing ? ConsoleArgErrorType.ValueParsingError : ConsoleArgErrorType.ValueAssignmentError,
								e
							));
						}
					}
				};
			}
		}

		public void WriteOptionDescriptions(TextWriter writer) => CreateOptionsFor(new ConsoleArgsContext()).WriteOptionDescriptions(writer);
	}
}
