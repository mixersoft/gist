using System;
using System.Collections.Generic;
using Snaphappi.Properties;

namespace Snaphappi
{
	public class WFModel : IWFModel
	{
		#region data
		
		private readonly ITaskControlService controlService;
		private readonly ITaskUploadService  uploadService;
		private readonly IPhotoLoader          photoLoader;
		private readonly string                authToken;

		private readonly HashSet<string> files
			= new HashSet<string>();

		private readonly HashSet<string> folders	
			= new HashSet<string>();

		private readonly Dictionary<string, int> uploadedFileCounts
			= new Dictionary<string,int>();

		#endregion // data

		public WFModel
			( ITaskControlService controlService
			, ITaskUploadService  uploadService
			, IPhotoLoader          photoLoader
			, string                authToken
			)
		{
			this.controlService = controlService;
			this.uploadService  = uploadService;
			this.photoLoader    = photoLoader;
			this.authToken      = authToken;
		}

		#region IWFModel Members

		public void FetchFiles(string folderPath)
		{
			AddFiles(controlService.GetFiles(folderPath));
		}

		public void FetchFolders()
		{
			var folders = controlService.GetWatchedFolders();
			if (folders.Length == 0)
			{
				FolderListEmpty();
			}
			else
			{
				AddFolders(folders);
				uploadService.ScheduleAction(() => AllFolderUploadsComplete());
			}
		}

		public int GetFileCount(string folderPath)
		{
			int count = 0;
			uploadedFileCounts.TryGetValue(folderPath, out count);
			return count;
		}

		public void ScheduleFolderUploadCompletionEvent(string folderPath)
		{
			uploadService.ScheduleAction(() => FolderUploadComplete(folderPath));
		}

		public void UploadFile(string folderPath, string filePath)
		{
			IncrementUploadedFileCount(folderPath);

			if (files.Contains(filePath.ToUpperInvariant()))
				return;
			uploadService.UploadFile(folderPath, filePath, () => photoLoader.GetPreview(filePath));
		}

		public void UnscheduleWatcher()
		{
			SystemScheduler.UnscheduleWatcher(authToken);
		}

		public event Action AllFolderUploadsComplete = delegate {};

		public event Action AuthTokenRejected
		{
			add
			{
				controlService.AuthTokenRejected += value;
				uploadService.AuthTokenRejected  += value;
			}
			remove
			{
				controlService.AuthTokenRejected += value;
				uploadService.AuthTokenRejected  += value;
			}
		}

		public event Action FolderListEmpty = delegate {};

		public event Action<string> FolderUploadComplete = delegate {};

		public event Action<string> FolderAdded = delegate {};

		public event Action<string, string> DuplicateUpload
		{
			add    { uploadService.DuplicateUpload += value; }
			remove { uploadService.DuplicateUpload -= value; }
		}

		public event Action<string, string> UploadFailed
		{
			add    { uploadService.UploadFailed += value; }
			remove { uploadService.UploadFailed -= value; }
		}

		#endregion // IWFModel Members

		#region implementation

		private void AddFiles(string[] files)
		{
			foreach (var filePath in files)
			{
				var ucFilePath = filePath.ToUpperInvariant();
				if (!this.files.Contains(ucFilePath))
					this.files.Add(ucFilePath);
			}
		}

		private void AddFolders(string[] folders)
		{
			foreach (var folderPath in folders)
			{
				var ucFolderPath = folderPath.ToUpperInvariant();
				if (!this.folders.Contains(ucFolderPath))
				{
					this.folders.Add(ucFolderPath);
					FolderAdded(folderPath);
				}
			}
		}

		private void IncrementUploadedFileCount(string folderPath)
		{
			int count = 0;
			uploadedFileCounts.TryGetValue(folderPath, out count);
			uploadedFileCounts[folderPath] = count + 1;
		}

		#endregion // implementation
	}
}
