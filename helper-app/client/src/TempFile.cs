using System;

namespace Snaphappi
{
	public class TempFile : IDisposable
	{
		public TempFile()
		{
			Path = System.IO.Path.GetTempFileName();
		}

		public string Path { get; private set; }

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				System.IO.File.Delete(Path);
			}
			catch (Exception)
			{
				// inability to delete the file does not compromise the program
			}
		}

		#endregion
	}
}
