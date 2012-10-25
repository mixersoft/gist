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
		#region interface

		/// <summary>
		/// Allocates a console and resets the standard stream handles.
		/// </summary>
		public static void Alloc()
		{
			if (!AllocConsole())
				throw new Win32Exception();
			SetStdHandle(StdHandle.Output, GetConsoleStandardOutput());
			SetStdHandle(StdHandle.Input,  GetConsoleStandardInput());
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

		#endregion

		#region helper functions

		private static IntPtr GetConsoleStandardInput()
		{
			var handle = CreateFile
				( "CONIN$"
				, DesiredAccess.GenericRead | DesiredAccess.GenericWrite
				, FileShare.ReadWrite
				, IntPtr.Zero
				, FileMode.Open
				, FileAttributes.Normal
				, IntPtr.Zero
				);
			if (handle == InvalidHandleValue)
				throw new Win32Exception();
			return handle;
		}

		private static IntPtr GetConsoleStandardOutput()
		{
			var handle = CreateFile
				( "CONOUT$"
				, DesiredAccess.GenericWrite | DesiredAccess.GenericWrite
				, FileShare.ReadWrite
				, IntPtr.Zero
				, FileMode.Open
				, FileAttributes.Normal
				, IntPtr.Zero
				);
			if (handle == InvalidHandleValue)
				throw new Win32Exception();
			return handle;
		}

		#endregion

		#region PInvoke

		[DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle(StdHandle nStdHandle);
		
		[DllImport("kernel32.dll")]
		static extern bool SetConsoleTitle(string lpConsoleTitle);

		[DllImport("kernel32.dll")]
		static extern bool SetStdHandle(StdHandle nStdHandle, IntPtr hHandle);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern IntPtr CreateFile
			(                               string         lpFileName
			, [MarshalAs(UnmanagedType.U4)] DesiredAccess  dwDesiredAccess
			, [MarshalAs(UnmanagedType.U4)] FileShare      dwShareMode
			,                               IntPtr         lpSecurityAttributes
			, [MarshalAs(UnmanagedType.U4)] FileMode       dwCreationDisposition
			, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes
			,                               IntPtr         hTemplateFile
			);

		private enum DesiredAccess
        {
            GenericRead  = unchecked((int)0x80000000),
            GenericWrite = 0x40000000
        }

		private enum StdHandle : int
		{
			Input  = -10,
			Output = -11,
			Error  = -12
		}

		private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

		#endregion
	}
}
