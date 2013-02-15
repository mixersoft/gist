using System;
using System.IO;

namespace Snaphappi
{
	public class Logger
	{
		public void RecordException(Exception e)
		{
			using (var writer = File.AppendText(FilePath))
			{
				writer.WriteLine(DateTime.Now);
				writer.WriteLine(e);
				writer.WriteLine();
			}
		}

		private string FilePath
		{
			get
			{
				var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				var dir = Path.Combine(appData, "Snaphappi");
				Directory.CreateDirectory(dir);
				return Path.Combine(dir, "log.txt");
			}
		}
	}
}
