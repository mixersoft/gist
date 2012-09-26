using System;

namespace Snaphappi
{
	public interface IFileLister
	{
		void AddFolder(string folderPath);

		void Start();

		void Stop();

		event Action<string, string> FileFound;

		event Action<string> FolderNotFound;
	}
}
