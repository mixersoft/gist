using System;

namespace Snaphappi
{
	public interface ITaskInfoService
	{
		/// <summary>
		/// Start polling the server.
		/// </summary>
		/// <param name="period">Polling period, in milliseconds.</param>
		void StartPolling(int period);

		void StopPolling();

		event Action AuthTokenRejected;
		event Action FilesUpdated;
		event Action FoldersUpdated;
		event Action TaskCancelled;
	}
}
