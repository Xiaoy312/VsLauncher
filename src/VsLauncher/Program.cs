using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using VsLauncher.Arguments;
using VsLauncher.Helpers;

namespace VsLauncher
{
	public partial class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				var context = ConsoleArgsContext.Parse(args);

				if (context.HasError)
				{
					Console.Error.WriteLine(context.Errors[0].Message);
					Environment.Exit(-1);
				}
				else if (context.IsHelp)
				{
					context.WriteOptionDescriptions(Console.Out);
				}
				else
				{
					context.Path ??= Environment.CurrentDirectory;
					var launcher = new Launcher(Config, context);

					launcher.Launch();
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e);
				Environment.Exit(-1);
			}
		}
		
		private static IConfiguration Config = new ConfigurationBuilder()
			.SetBasePath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName))
			.AddYamlFile("config.yml")
			.Build();
	}
}
