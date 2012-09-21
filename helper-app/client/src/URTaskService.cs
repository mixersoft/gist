using Thrift.Protocol;
using Thrift.Transport;
using Snaphappi.API;
using System;
using System.Threading;

namespace Snaphappi
{
	public class URTaskService : IURTaskService
	{
		#region data

		private readonly URTask.Client task;

		private readonly TaskID id;

		private Timer timer;

		#endregion

		#region interface

		public URTaskService(int taskID, string sessionID)
		{
			this.id = new TaskID();
			id.Task    = taskID;
			id.Session = sessionID;
			id.__isset.Session = true;
			id.__isset.Task    = true;

			var uri = new Uri(""); // FIXME
			task = new URTask.Client(new TBinaryProtocol(new THttpClient(uri)));

			timer = new Timer(OnTimer);
		}

		#endregion

		#region IURTaskService Members

		public string[] GetFolders()
		{
			return task.GetFolders(id).ToArray();
		}

		public void StartPolling(int period)
		{
			timer.Change(0, period);
		}

		public void StopPolling()
		{
			timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		public UploadResampledTaskStatus GetStatus()
		{
			throw new NotImplementedException();
		}

		public event Action TaskCancelled;

		#endregion

		#region implementation

		private void OnTimer(Object o)
		{
			var state = task.GetState(id);
			if (state.IsCancelled)
				TaskCancelled();
		}

		#endregion
	}
}
