using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VsLauncher.Arguments
{
	public partial class ConsoleArgsContext
	{
		private ConsoleArgsContext() { }

		#region Parsing Error

		public IList<ConsoleArgError> Errors { get; } = new List<ConsoleArgError>();

		public bool HasError => Errors.Any();

		#endregion

		public bool IsHelp { get; set; }

		public bool IsVerbose { get; private set; }

		public string Path { get; set; }

		public string? VsVersion { get; set; }

		public bool IncludeSlnf { get; set; }

		public string? SlnfHint { get; set; }
	}
}
