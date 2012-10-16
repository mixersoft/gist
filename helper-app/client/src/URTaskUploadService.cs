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
			public readonly string       Folder;
			public readonly string       Path;
			public readonly Func<byte[]> LoadFile;

			public UploadTask(string folder, string path, Func<byte[]> LoadFile)
			{
				this.Folder   = folder;
				this.Path     = path;
				this.LoadFile = LoadFile;
			}
		}

		#endregion

		#region data

		private readonly Task.Client taskUpload;

		private readonly TaskID id;

		private Thread uploadThread;

		private BlockingQueue<UploadTask> uploadTaskQueue;

		#endregion

		#region interface

		public URTaskUploadService(string authToken, string sessionID, Uri uri)
		{
			this.id = ApiHelper.MakeTaskID(authToken, sessionID);
			
			taskUpload = new Task.Client(new TBinaryProtocol(new THttpClient(uri)));

			uploadTaskQueue = new BlockingQueue<UploadTask>();

			uploadThread = new Thread(UploadProc);
			uploadThread.Start();
		}

		#endregion

		#region IURUploadService Members

		public void UploadFile(string folder, string path, Func<byte[]> LoadFile)
		{
			uploadTaskQueue.Enqueue(new UploadTask(folder, path, LoadFile));
		}

		public event Action<string, string> UploadFailed;

		#endregion

		#region implementation

		private void UploadProc()
		{
			foreach (var uploadTask in uploadTaskQueue)
			{
				try
				{
					taskUpload.UploadFile(id, uploadTask.Path, uploadTask.LoadFile());
				}
				catch (Exception)
				{
					UploadFailed(uploadTask.Folder, uploadTask.Path);
				}
			}
		}

		#endregion
	}
}
