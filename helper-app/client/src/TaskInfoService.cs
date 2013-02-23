using Snaphappi.API;
using Snaphappi.Properties;
using System;
using System.Timers;
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
		private bool  stopPolling;

		private int fileUpdateCount         = 0;
		private int folderUpdateCount       = 0;
		private int uploadTargetUpdateCount = 0;

		#endregion

		#region interface

		public TaskInfoService(TaskID taskID, Uri uri)
		{
			this.id = ApiHelper.ConvertTaskID(taskID);
			
			var transport = new THttpClient(uri);
			transport.MaxRetryCount = Settings.Default.ConnectionRetryCount;
			task = new Task.Client(new TBinaryProtocol(transport));

			timer = new Timer();
			timer.Elapsed += OnTimer;
			timer.AutoReset = false;
		}

		#endregion

		#region IURTaskService Members

		public void StartPolling(int period)
		{
			timer.Interval = period;
			OnTimer(null, null);
			stopPolling = false;
			timer.Start();
		}

		public void StopPolling()
		{
			stopPolling = true;
			timer.Stop();
		}

		public event Action AuthTokenRejected    = delegate {};
		public event Action FilesUpdated         = delegate {};
		public event Action FoldersUpdated       = delegate {};
		public event Action TaskCancelled        = delegate {};
		public event Action UploadTargetsUpdated = delegate {};

		#endregion

		#region implementation

		private void OnTimer(object o, ElapsedEventArgs e)
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
			if (state.FilesToUploadUpdateCount > uploadTargetUpdateCount)
			{
				uploadTargetUpdateCount = state.FilesToUploadUpdateCount;
				UploadTargetsUpdated();
			}
			if (!stopPolling)
				timer.Start();
		}

		private TaskState SafeGetState()
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
