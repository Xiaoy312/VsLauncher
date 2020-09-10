using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VsLauncher.Helpers
{
	public static class DirectoryHelper
	{
		public static IEnumerable<string> EnumerateAllNestedDirectories(string path) => EnumerateAllNestedDirectories(path, _ => true);
		public static IEnumerable<string> EnumerateAllNestedDirectories(string path, Func<string, bool> predicate)
		{
			foreach (var directory in Directory.EnumerateDirectories(path))
			{
				if (predicate(directory))
				{
					yield return directory;
					foreach (var subdirectory in EnumerateAllNestedDirectories(directory, predicate))
					{
						yield return subdirectory;
					}
				}
			}
		}
	}
}
