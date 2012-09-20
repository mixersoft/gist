using System;

namespace Snaphappi
{
	public interface IURTaskService
	{
		string[] GetFolders();

		UploadResampledTaskStatus GetStatus();

		event Action TaskCancelled;
	}
}
