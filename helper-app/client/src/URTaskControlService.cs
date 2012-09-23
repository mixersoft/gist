using Snaphappi.API;
using System;
using Thrift.Protocol;
using Thrift.Transport;

namespace Snaphappi
{
	public class URTaskControlService : IURTaskControlService
	{
		#region data

		private readonly URTaskControl.Client task;

		private readonly TaskID id;
		
		#endregion

		#region interface

		public URTaskControlService(int taskID, string sessionID)
		{
			this.id = ApiHelper.MakeTaskID(taskID, sessionID);

			var uri = new Uri(""); // FIXME
			task = new URTaskControl.Client(new TBinaryProtocol(new THttpClient(uri)));
		}

		#endregion

		#region IURTaskControlService Members

		public string[] GetFolders()
		{
			lock (task)
				return task.GetFolders(id).ToArray();
		}

		#endregion
	}
}
