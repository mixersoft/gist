using System;
using System.Collections.Generic;
using System.Management;

namespace Snaphappi
{
	public class Wmi
	{
		/// <summary>
		/// Using WMI to fetch the command line that started all instances of a process
		/// </summary>
		/// <param name="processName">Image name, e.g. WebDev.WebServer.exe</param>
		/// adapted from http://stackoverflow.com/questions/504208/how-to-read-command-line-arguments-of-another-process-in-c/504378%23504378
		/// original code by http://stackoverflow.com/users/61396/xcud
		public static IEnumerable<string> GetCommandLines(string processName)
		{
			var results = new List<string>();
 
			var wmiQuery = string.Format("select CommandLine from Win32_Process where Name='{0}'", processName);
 
			using (var searcher = new ManagementObjectSearcher(wmiQuery))
			{
				using (var retObjectCollection = searcher.Get())
				{
					foreach (var retObject in retObjectCollection)
						results.Add((string)retObject["CommandLine"]);
				}
			}

			return results;
		}
	}
}
