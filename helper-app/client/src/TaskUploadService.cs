using Thrift;
using Thrift.Protocol;
using Thrift.Transport;
using Snaphappi.API;
using System;
using System.Threading;

namespace Snaphappi
{
	public class URTaskUploadService : ITaskUploadService
	{
		#region data

		private readonly Task.Client task;

		private readonly Snaphappi.API.TaskID id;

		private Thread uploadThread;

		private BlockingQueue<Action> taskQueue;

		#endregion

		#region interface

		public URTaskUploadService(TaskID taskID, Uri uri)
		{
			this.id = ApiHelper.ConvertTaskID(taskID);
			
			task = new Task.Client(new TBinaryProtocol(new THttpClient(uri)));

			taskQueue = new BlockingQueue<Action>();

			uploadThread = new Thread(UploadProc);
			uploadThread.Name = "TaskUpload";
			uploadThread.Start();
		}

		#endregion

		#region IURUploadService Members

		public void UploadFile(string folder, string path, Func<byte[]> LoadFile)
		{
			taskQueue.Enqueue(() => SafeUploadFile(folder, path, LoadFile));
		}

		public void ScheduleAction(Action action)
		{
			taskQueue.Enqueue(action);
		}
		
		public event Action                 AuthTokenRejected = delegate {};
		public event Action<string, string> DuplicateUpload   = delegate {};
		public event Action<string, string> UploadFailed      = delegate {};

		#endregion

		#region implementation

		private void UploadProc()
		{
			foreach (var PerformTask in taskQueue)
				PerformTask();
		}

		private void SafeUploadFile(string folder, string path, Func<byte[]> LoadFile)
		{
			try
			{
				var info = new UploadInfo();
				info.UploadType = UploadType.Preview;
				info.__isset.UploadType = true;

				task.UploadFile(id, path, LoadFile(), info);
			}
			catch (Snaphappi.API.SystemException e)
			{
				switch (e.ErrorCode)
				{
					case ErrorCode.DataConflict:
						DuplicateUpload(folder, path);
						break;
					case ErrorCode.InvalidAuth:
						AuthTokenRejected();
						break;
					default:
						throw;
				}
			}
			catch (TApplicationException)
			{
				UploadFailed(folder, path);
			}
		}

		#endregion
	}
}
