using System;
using System.Diagnostics;
using System.IO;

namespace Launcher
{
	class Launcher
	{
		static int Main(string[] args)
		{
			Environment.CurrentDirectory = AssemblyDirectory;
			var process = CreateProcess(args);
			if (process == null)
				return 0;
			process.WaitForExit();
			return process.ExitCode;
		}

		static Process CreateProcess(string[] args)
		{
			var info = new ProcessStartInfo();
			info.CreateNoWindow  = true;
			info.UseShellExecute = false;
			switch (args.Length)
			{
				case 1:
					info.FileName = args[0];
					return Process.Start(info);
				case 2:
					info.FileName  = args[0];
					info.Arguments = args[1];
					return Process.Start(info);
				default:
					return null;
			}
		}

		static string AssemblyDirectory
		{
			get { return Path.GetDirectoryName(typeof(Launcher).Assembly.Location); }
		}
	}
}
