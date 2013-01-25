using Thrift;
using Thrift.Protocol;
using Thrift.Transport;
using Snaphappi.API;
using System;
using System.Threading;

namespace Snaphappi
{
	public class TaskUploadService : ITaskUploadService
	{
		#region data

		private readonly Task.Client task;

		private readonly Snaphappi.API.TaskID id;

		private Thread uploadThread;

		private BlockingQueue<Action> taskQueue = new BlockingQueue<Action>();

		#endregion

		#region interface

		public TaskUploadService(TaskID taskID, Uri uri)
		{
			this.id = ApiHelper.ConvertTaskID(taskID);
						
			task = new Task.Client(new TBinaryProtocol(new THttpClient(uri)));

			uploadThread = new Thread(UploadProc);
			uploadThread.Name = "TaskUpload";
			uploadThread.Start();
		}

		#endregion

		#region IURUploadService Members

		public void ScheduleAction(Action action)
		{
			taskQueue.Enqueue(action);
		}

		public void Stop()
		{
			taskQueue.Enqueue(null);
		}

		public void UploadFile
			( string       folder
			, string       path
			, UploadType   uploadType
			, Func<byte[]> LoadFile
			)
		{
			taskQueue.Enqueue(() => SafeUploadFile(folder, null, path, uploadType, LoadFile));
		}

		public void UploadFile
			( string       folder
			, ImageID      imageID
			, string       path
			, UploadType   uploadType
			, Func<byte[]> LoadFile
			)
		{
			taskQueue.Enqueue(() => SafeUploadFile(folder, imageID, path, uploadType, LoadFile));
		}
		
		public event Action                 AuthTokenRejected = delegate {};
		public event Action<string, string> DuplicateUpload   = delegate {};
		public event Action<string, string> FileNotFound      = delegate {};
		public event Action<string, string> UploadFailed      = delegate {};

		#endregion

		#region implementation

		private void UploadProc()
		{
			foreach (var PerformTask in taskQueue)
			{
				if (PerformTask == null)
					break;
				PerformTask();
			}
		}

		private void SafeUploadFile
			( string       folder
			, ImageID      imageID
			, string       path
			, UploadType   uploadType
			, Func<byte[]> LoadFile
			)
		{
			try
			{
				var info = new UploadInfo();
				info.UploadType = MapUploadType(uploadType);
				info.__isset.UploadType = true;
				if (imageID != null)
				{
					info.ImageID = imageID.Data;
					info.__isset.ImageID = true;
				}

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
			catch (System.IO.FileNotFoundException)
			{
				FileNotFound(folder, path);
			}
		}

		#endregion

		private API.UploadType MapUploadType(UploadType uploadType)
		{
			switch (uploadType)
			{
				case UploadType.Original: return API.UploadType.Original;
				case UploadType.Preview:  return API.UploadType.Preview;
			}
			throw new ArgumentException("uploadType");
		}
	}
}
