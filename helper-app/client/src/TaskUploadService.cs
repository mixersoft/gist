﻿using Thrift;
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

		public void UploadFile
			( string       folder
			, string       path
			, UploadType   uploadType
			, Func<byte[]> LoadFile
			)
		{
			taskQueue.Enqueue(() => SafeUploadFile(folder, path, uploadType, LoadFile));
		}

		public void ScheduleAction(Action action)
		{
			taskQueue.Enqueue(action);
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
				PerformTask();
		}

		private void SafeUploadFile
			( string       folder
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
