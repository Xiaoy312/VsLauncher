using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsLauncher.Extensions
{
	public static class FluentExtensions
	{
		public static TResult Apply<T, TResult>(this T x, Func<T, TResult> selector)
		{
			return selector(x);
		}
	}
}
