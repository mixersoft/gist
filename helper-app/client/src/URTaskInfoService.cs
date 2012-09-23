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

		#endregion

		#region interface

		public URTaskInfoService(int taskID, string sessionID)
		{
			this.id = ApiHelper.MakeTaskID(taskID, sessionID);

			var uri = new Uri(""); // FIXME
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
