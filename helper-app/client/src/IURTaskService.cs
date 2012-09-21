using System;

namespace Snaphappi
{
	public interface IURTaskService
	{
		string[] GetFolders();
		
		/// <summary>
		/// Start polling the server.
		/// </summary>
		/// <param name="period">Polling period, in milliseconds.</param>
		void StartPolling(int period);

		void StopPolling();

		UploadResampledTaskStatus GetStatus();

		event Action TaskCancelled;
	}
}
