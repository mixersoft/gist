using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Snaphappi
{
	public class Wmi
	{
		/// <summary>
		/// Using WMI to fetch the command line that started all instances of a process
		/// </summary>
		/// <param name="processName">Image name, e.g. WebDev.WebServer.exe</param>
		/// Adapted from: http://stackoverflow.com/questions/504208/how-to-read-command-line-arguments-of-another-process-in-c/504378%23504378
		/// Original code by: http://stackoverflow.com/users/61396/xcud
		public static IEnumerable<string[]> GetCommandLines()
		{
			var wmiQuery = string.Format
				( "SELECT CommandLine FROM Win32_Process WHERE Name = '{0}' AND ProcessID != {1}"
				, Path.GetFileName(typeof(Wmi).Assembly.Location)
				, Process.GetCurrentProcess().Id
				);
			using (var searcher = new ManagementObjectSearcher(wmiQuery))
			{
				using (var retObjectCollection = searcher.Get())
				{
					foreach (var retObject in retObjectCollection)
						yield return CommandLineToArgs((string)retObject["CommandLine"]);
				}
			}
		}

		/// Adapted from: http://stackoverflow.com/a/749653/49329
		private static string[] CommandLineToArgs(string commandLine)
		{
			int argc;
			var argv = CommandLineToArgvW(commandLine, out argc);
			if (argv == IntPtr.Zero)
				throw new Win32Exception();
			try
			{
				var args = new string[argc];
				for (var i = 0; i < args.Length; i++)
					args[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(argv, i * IntPtr.Size));
				return args;
			}
			finally
			{
				Marshal.FreeHGlobal(argv);
			}
		}

		[DllImport("shell32.dll", SetLastError = true)]
		static extern IntPtr CommandLineToArgvW
			( [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine
			, out int pNumArgs
			);

	}
}
