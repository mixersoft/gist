using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace Snaphappi
{
	public static class ConsoleHelper
	{
		public static void Alloc()
		{
			if (!AllocConsole())
				throw new Win32Exception();
		}

		public static StreamReader StandardInput
		{
			get
			{
				return new StreamReader(
					new FileStream
						( new SafeFileHandle(GetStdHandle(StdHandle.Input), false)
						, FileAccess.Read
						)
					);
			}
		}

		public static StreamWriter StandardOutput
		{
			get
			{
				return new StreamWriter
					( new FileStream
						( new SafeFileHandle(GetStdHandle(StdHandle.Output), false)
						, FileAccess.Write
						)
					);
			}
		}

		public static StreamWriter StandardError
		{
			get
			{
				return new StreamWriter(
					new FileStream
						( new SafeFileHandle(GetStdHandle(StdHandle.Error), false)
						, FileAccess.Write
						)
					);
			}
		}

		public static bool Exists
		{
			get { return IntPtr.Zero != GetConsoleWindow(); }
		}

		public static string Title
		{
			set { SetConsoleTitle(value); }
		}

		[DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle(StdHandle nStdHandle);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();
		
		[DllImport("kernel32.dll")]
		static extern bool SetConsoleTitle(string lpConsoleTitle);

		private enum StdHandle : int
		{
			Input  = -10,
			Output = -11,
			Error  = -12
		}
	}
}
