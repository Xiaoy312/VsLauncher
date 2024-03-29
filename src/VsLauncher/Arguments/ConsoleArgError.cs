﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VsLauncher.Arguments
{
	public class ConsoleArgError
	{
		public ConsoleArgError(string argument, ConsoleArgErrorType type, Exception e = null)
		{
			Argument = argument;
			Type = type;
			Exception = e;
		}

		public ConsoleArgErrorType Type { get; set; }

		public string Argument { get; set; }

		public Exception Exception { get; set; }

		public string Message => Type switch
		{
			ConsoleArgErrorType.UnrecognizedArgument => "unrecognized argument: " + Argument,
			ConsoleArgErrorType.ValueAssignmentError => "error while trying to assign value: " + Argument,
			ConsoleArgErrorType.ValueParsingError => "error while trying to parse value: " + Argument,

			_ => $"{Type}: " + Argument,
		};
	}
}
