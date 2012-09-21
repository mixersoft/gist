using Thrift.Protocol;
using Thrift.Transport;
using Snaphappi.API;
using System;

namespace Snaphappi
{
	public class URUploadService : IURUploadService
	{
		#region data

		private readonly URTaskUpload.Client taskUpload;

		private readonly TaskID id;

		#endregion

		#region interface

		public URUploadService(int taskID, string sessionID)
		{
			this.id = new TaskID();
			id.Task    = taskID;
			id.Session = sessionID;
			id.__isset.Session = true;
			id.__isset.Task    = true;
			
			var uri = new Uri(""); // FIXME
			taskUpload = new URTaskUpload.Client(new TBinaryProtocol(new THttpClient(uri)));
		}

		#endregion

		#region IURUploadService Members

		public void UploadFile(string path)
		{
			throw new NotImplementedException();
		}

		public event Action<string> UploadFailed;

		#endregion
	}
}
