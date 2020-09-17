using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using VsLauncher.Helpers;

namespace VsLauncher
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				if (args.Length > 2) throw new ArgumentOutOfRangeException("Too many argument.\nsyntax: VS [path] [devenv]");

				var solution = FindSolutionFile(args.ElementAtOrDefault(0) ?? Environment.CurrentDirectory);
				var environment = ParseEnvironment(args.ElementAtOrDefault(1));

				Process.Start(environment, $"\"{solution}\"");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		static readonly IConfiguration Config = new ConfigurationBuilder()
			.SetBasePath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName))
			.AddYamlFile("config.yml")
			.Build();

		static string FindSolutionFile(string path)
		{
			var matches = DirectoryHelper.EnumerateAllNestedDirectories(path, x => ".git,.vs,Components,bin,obj".Split(',').All(y => !x.EndsWith(y)))
				.Prepend(path)
				.SelectMany(x => Directory.GetFiles(x, "*.sln"))
				.ToArray();
			if (matches.Length > 1)
			{
				Console.WriteLine(string.Join("\n", matches
					.Select((x, i) => $"    [{i + 1}]: {x}")
					.Prepend("Multiple solution files found. Please select one:")
				));

				// failing with format or index out of range are valid exit condition
				var solution = matches[int.Parse(Console.ReadLine()) - 1];

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
				throw new InvalidOperationException("Solution file not found in: " + path);
			}
		}

		static string ParseEnvironment(string version = null)
		{
			var devenvs = Config.GetSection("devenv").Get<Dictionary<string, string>>();
			if (!devenvs.Any())
			{
				throw new InvalidOperationException("`devenv` are not specified in config.yml");
			}

			if (version == null)
			{
				var path = devenvs.Values.FirstOrDefault();

				Console.WriteLine("Selecting first devenv by default: " + path);
				return path;
			}
			else if (devenvs.TryGetValue(version, out var path))
			{
				Console.WriteLine($"Selecting devenv '{version}': {path}");
				return path;
			}
			else
			{
				throw new ArgumentException($"devenv '{version}' is not defined in config.yml");
			}
		}
	}
}
