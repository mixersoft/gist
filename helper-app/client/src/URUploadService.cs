using Thrift.Protocol;
using Thrift.Transport;
using Snaphappi.API;
using System;
using System.Threading;

namespace Snaphappi
{
	public class URTaskUploadService : IURTaskUploadService
	{
		#region nested types

		private struct UploadTask
		{
			public readonly string       Path;
			public readonly Func<byte[]> LoadFile;

			public UploadTask(string path, Func<byte[]> LoadFile)
			{
				this.Path     = path;
				this.LoadFile = LoadFile;
			}
		}

		#endregion

		#region data

		private readonly URTaskUpload.Client taskUpload;

		private readonly TaskID id;

		private Thread uploadThread;

		private BlockingQueue<UploadTask> uploadTaskQueue;

		#endregion

		#region interface

		public URTaskUploadService(int taskID, string sessionID)
		{
			this.id = ApiHelper.MakeTaskID(taskID, sessionID);
			
			var uri = new Uri(""); // FIXME
			taskUpload = new URTaskUpload.Client(new TBinaryProtocol(new THttpClient(uri)));

			uploadTaskQueue = new BlockingQueue<UploadTask>();

			uploadThread = new Thread(UploadProc);
			uploadThread.Start();
		}

		#endregion

		#region IURUploadService Members

		public void UploadFile(string path, Func<byte[]> LoadFile)
		{
			uploadTaskQueue.Enqueue(new UploadTask(path, LoadFile));
		}

		public event Action<string> UploadFailed;

		#endregion

		#region implementation

		private void UploadProc()
		{
			for (;;)
			{
				var uploadTask = uploadTaskQueue.Dequeue();
				try
				{
					taskUpload.UploadFile(id, uploadTask.Path, uploadTask.LoadFile());
				}
				catch (Exception)
				{
					UploadFailed(uploadTask.Path);
				}
			}
		}

		#endregion
	}
}
