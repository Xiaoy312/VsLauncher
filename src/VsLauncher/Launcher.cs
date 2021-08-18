using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VsLauncher.Arguments;
using VsLauncher.Extensions;
using VsLauncher.Helpers;

namespace VsLauncher
{
	public class Launcher
	{
		private readonly IConfiguration _config;
		private readonly ConsoleArgsContext _context;

		public Launcher(IConfiguration config, ConsoleArgsContext context)
		{
			_config = config;
			_context = context;
		}

		public void Launch()
		{
			var solution = FindSolutionFile();
			var vs = GetVsPath();
			
			Process.Start(vs, $"\"{solution}\"");
		}

		public string FindSolutionFile()
		{
			var matches = DirectoryHelper.EnumerateAllNestedDirectories(_context.Path, x => ".git,.vs,Components,bin,obj".Split(',').All(y => !x.EndsWith(y)))
				.Prepend(_context.Path)
				.SelectMany(x => Directory.GetFiles(x))
				.Where(x => MatchSln(x) || (_context.IncludeSlnf && MatchSlnf(x)))
				.ToArray();
			if (matches.Length > 1)
			{
				Console.WriteLine(string.Join("\n", matches
					.Select((x, i) => $"    [{i + 1}]: {x}")
					.Prepend("Multiple solution files found. Please select one:")
				));

				// failing with format or index out of range are valid exit condition
				var solution = matches[int.Parse(Console.ReadLine()!) - 1];

				Console.WriteLine("Selected solution: " + solution);
				return solution;
			}
			if (matches.Length == 1)
			{
				Console.WriteLine("Found solution: " + matches[0]);
				return matches[0];
			}
			else
			{
				throw new InvalidOperationException("Solution file not found in: " + _context.Path);
			}

			bool MatchSln(string x) => Path.GetExtension(x).Equals(".sln", StringComparison.OrdinalIgnoreCase);
			bool MatchSlnf(string x) => Path.GetExtension(x).Equals(".slnf", StringComparison.OrdinalIgnoreCase) &&
				(_context.SlnfHint?.Apply(y => Path.GetFileNameWithoutExtension(x).Contains(y, StringComparison.OrdinalIgnoreCase)) ?? true);
		}

		public string GetVsPath()
		{
			var devenvs = _config.GetSection("devenv").Get<Dictionary<string, string>>();
			if (!devenvs.Any())
			{
				throw new InvalidOperationException("`devenv` are not specified in config.yml");
			}

			if (_context.VsVersion == null)
			{
				var path = devenvs.Values.First();

				Console.WriteLine("Selecting first devenv by default: " + path);
				return path;
			}
			else if (devenvs.TryGetValue(_context.VsVersion, out var path))
			{
				Console.WriteLine($"Selecting devenv '{_context.VsVersion}': {path}");
				return path;
			}
			else
			{
				throw new ArgumentException($"devenv '{_context.VsVersion}' is not defined in config.yml");
			}
		}
	}
}
