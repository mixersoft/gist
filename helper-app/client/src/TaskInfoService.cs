using Snaphappi.API;
using System;
using System.Threading;
using Thrift.Protocol;
using Thrift.Transport;

namespace Snaphappi
{
	public class TaskInfoService : ITaskInfoService
	{
		#region data

		private readonly Task.Client task;

		private readonly Snaphappi.API.TaskID id;

		private Timer timer;

		private int fileUpdateCount   = 0;
		private int folderUpdateCount = 0;

		#endregion

		#region interface

		public TaskInfoService(TaskID taskID, Uri uri)
		{
			this.id = ApiHelper.ConvertTaskID(taskID);

			task = new Task.Client(new TBinaryProtocol(new THttpClient(uri)));

			timer = new Timer(OnTimer);
		}

		#endregion

		#region IURTaskService Members

		public void StartPolling(int period)
		{
			OnTimer(null);
			timer.Change(period, period);
		}

		public void StopPolling()
		{
			timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		public event Action AuthTokenRejected = delegate {};
		public event Action FilesUpdated      = delegate {};
		public event Action FoldersUpdated    = delegate {};
		public event Action TaskCancelled     = delegate {};

		#endregion

		#region implementation

		private void OnTimer(Object o)
		{
			var state = SafeGetState();
			if (state.IsCancelled)
				TaskCancelled();
			if (state.FileUpdateCount > fileUpdateCount)
			{
				fileUpdateCount = state.FileUpdateCount;
				FilesUpdated();
			}
			if (state.FolderUpdateCount > folderUpdateCount)
			{
				folderUpdateCount = state.FolderUpdateCount;
				FoldersUpdated();
			}
		}

		private URTaskState SafeGetState()
		{
			try
			{
				return task.GetState(id);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		#endregion
	}
}
