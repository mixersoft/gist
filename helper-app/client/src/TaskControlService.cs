using Snaphappi.API;
using System;
using Thrift.Protocol;
using Thrift.Transport;

namespace Snaphappi
{
	public class URTaskControlService : ITaskControlService
	{
		#region data

		private readonly Task.Client task;

		private readonly Snaphappi.API.TaskID id;
		
		#endregion

		#region interface

		public URTaskControlService(TaskID taskID, Uri uri)
		{
			this.id = ApiHelper.ConvertTaskID(taskID);

			task = new Task.Client(new TBinaryProtocol(new THttpClient(uri)));
		}

		public event Action AuthTokenRejected = delegate {};

		#endregion

		#region IURTaskControlService Members

		public string[] GetFiles(string folder)
		{
			try
			{
				lock (task)
					return task.GetFiles(id, folder).ToArray();
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public string[] GetFolders()
		{
			try
			{
				lock (task)
					return task.GetFolders(id).ToArray();
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public string[] GetWatchedFolders()
		{
			try
			{
				lock (task)
					return task.GetWatchedFolders(id).ToArray();
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public void ReportFolderNotFound(string folder)
		{
			try
			{
				lock (task)
					task.ReportFolderNotFound(id, folder);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public void ReportUploadFailed(string folder, string path)
		{
			try
			{
				lock (task)
					task.ReportUploadFailed(id, folder, path);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public void ReportFolderUploadComplete(string folder)
		{
			try
			{
				lock (task)
					task.ReportFolderUploadComplete(id, folder);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public void ReportFolderFileCount(string folder, int count)
		{
			try
			{
				lock (task)
					task.ReportFileCount(id, folder, count);
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
