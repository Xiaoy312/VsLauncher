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
				var context = ConsoleArgsContext.Parse(args, defaultPath: Environment.CurrentDirectory);

				if (context.HasError)
				{
					foreach (var error in context.Errors)
					{
						Console.Error.WriteLine(error.Message);
						if (error.Exception is not null)
						{
							Console.Error.WriteLine(error.Exception);
						}
					}
					Environment.Exit(-1);
				}
				else if (context.IsHelp)
				{
					context.WriteOptionDescriptions(Console.Out);
				}
				else
				{
					new Launcher(Config, context).Launch();
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
