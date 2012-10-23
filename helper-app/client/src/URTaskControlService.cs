using Snaphappi.API;
using System;
using Thrift.Protocol;
using Thrift.Transport;

namespace Snaphappi
{
	public class URTaskControlService : IURTaskControlService
	{
		#region data

		/// <summary>
		/// The generated task class. All access to it is serialized between threads.
		/// </summary>
		private readonly Task.Client task;

		private readonly Snaphappi.API.TaskID id;
		
		#endregion

		#region interface

		public URTaskControlService(TaskID taskID, Uri uri)
		{
			this.id = ApiHelper.ConvertTaskID(taskID);

			task = new Task.Client(new TBinaryProtocol(new THttpClient(uri)));
		}

		#endregion

		#region IURTaskControlService Members

		public string[] GetFiles(string folder)
		{
			lock (task)
				return task.GetFiles(id, folder).ToArray();
		}

		public string[] GetFolders()
		{
			lock (task)
				return task.GetFolders(id).ToArray();
		}

		public void ReportFolderNotFound(string folder)
		{
			lock (task)
				task.ReportFolderNotFound(id, folder);
		}

		public void ReportUploadFailed(string folder, string path)
		{
			lock (task)
				task.ReportUploadFailed(id, folder, path);
		}

		public void ReportFolderUploadComplete(string folder)
		{
			lock (task)
				task.ReportFolderUploadComplete(id, folder);
		}

		public void ReportFolderFileCount(string folder, int count)
		{
			lock (task)
				task.ReportFileCount(id, folder, count);
		}

		#endregion
	}
}
