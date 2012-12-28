using System;

namespace Snaphappi
{
	public class FileMatch
	{
		public readonly string OldLocation;
		public readonly string NewLocation;

		public FileMatch
			( string oldLocation
			, string newLocation
			)
		{
			OldLocation = oldLocation;
			NewLocation = newLocation;
		}
	}

	public interface IAsyncFileFinder
	{
		void Find(string filePath, int timestamp, int hash);

		void Stop();

		event Action<FileMatch> FileFound;
		event Action<string>    FileNotFound;
	}
}
