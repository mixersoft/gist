using Snaphappi.API;
using System;
using System.Threading;
using Thrift.Protocol;
using Thrift.Transport;

namespace Snaphappi
{
	public class URTaskInfoService : IURTaskInfoService
	{
		#region data

		private readonly URTaskInfo.Client task;

		private readonly TaskID id;

		private Timer timer;

		private int folderUpdateCount = 0;

		#endregion

		#region interface

		public URTaskInfoService(int taskID, string sessionID, Uri uri)
		{
			this.id = ApiHelper.MakeTaskID(taskID, sessionID);

			task = new URTaskInfo.Client(new TBinaryProtocol(new THttpClient(uri)));

			timer = new Timer(OnTimer);
		}

		#endregion

		#region IURTaskService Members

		public void StartPolling(int period)
		{
			timer.Change(0, period);
		}

		public void StopPolling()
		{
			timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		public event Action TaskCancelled;
		public event Action FoldersUpdated;

		#endregion

		#region implementation

		private void OnTimer(Object o)
		{
			var state = task.GetState(id);
			if (state.IsCancelled)
				TaskCancelled();
			if (state.FolderUpdateCount > folderUpdateCount)
			{
				folderUpdateCount = state.FolderUpdateCount;
				FoldersUpdated();
			}
		}

		#endregion
	}
}
