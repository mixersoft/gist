using System;

namespace Snaphappi
{
	public interface IFileLister
	{
		void SearchFolder(string folderPath);

		event Action<string, string> FileFound;

		event Action<string> FolderNotFound;

		event Action<string> FolderSearchComplete;
	}
}
