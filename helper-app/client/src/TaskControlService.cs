using Snaphappi.API;
using System;
using System.Linq;
using Thrift.Protocol;
using Thrift.Transport;

namespace Snaphappi
{
	public class TaskControlService : ITaskControlService
	{
		#region data

		private readonly Task.Client task;

		private readonly Snaphappi.API.TaskID id;
		
		#endregion

		#region interface

		public TaskControlService(TaskID taskID, Uri uri)
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

		public UploadTarget[] GetFilesToUpload()
		{
			try
			{
				lock (task)
					return task.GetFilesToUpload(id).Select(MapUploadTarget).ToArray();
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

		public int GetImageHash(ImageID imageID)
		{
			try
			{
				lock (task)
					return task.GetImageHash(id, imageID.Data);
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

		public void ReportFileNotFound(string folderPath, string filePath)
		{
			try
			{
				lock (task)
					task.ReportFileNotFound(id, folderPath, filePath);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public void ReportFileNotFoundByID(ImageID imageID)
		{
			try
			{
				lock (task)
					task.ReportFileNotFoundByID(id, imageID.Data);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public void ReportFolderNotFound(string folderPath)
		{
			try
			{
				lock (task)
					task.ReportFolderNotFound(id, folderPath);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public void ReportUploadFailed(string folderPath, string filePath)
		{
			try
			{
				lock (task)
					task.ReportUploadFailed(id, folderPath, filePath);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public void ReportUploadFailedByID(ImageID imageID)
		{
			try
			{
				lock (task)
					task.ReportUploadFailedByID(id, imageID.Data);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public void ReportFolderUploadComplete(string folderPath)
		{
			try
			{
				lock (task)
					task.ReportFolderUploadComplete(id, folderPath);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		public void ReportFolderFileCount(string folderPath, int count)
		{
			try
			{
				lock (task)
					task.ReportFileCount(id, folderPath, count);
			}
			catch (API.SystemException e)
			{
				if (e.ErrorCode == ErrorCode.InvalidAuth)
					AuthTokenRejected();
				throw;
			}
		}

		#endregion

		#region implementation

		private UploadTarget MapUploadTarget(API.UploadTarget uploadTarget)
		{
			return new UploadTarget
				( uploadTarget.FilePath
				, uploadTarget.ExifDateTime
				, new ImageID(uploadTarget.ImageID)
				);
		}

		#endregion // implementation
	}
}
