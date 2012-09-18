using System;

namespace Snaphappi
{
	public interface IFileLister
	{
		void UpdateFolders(string[] paths);

		void Start();

		void Stop();

		event Action<string> FileFound;
		event Action<string> FolderNotFound;

		event Action Finished;
	}
}
