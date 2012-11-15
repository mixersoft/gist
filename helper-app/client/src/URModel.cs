using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Snaphappi.Properties;

namespace Snaphappi
{
	public class URModel : IURModel
	{
		#region data
		
		private readonly ITaskControlService controlService;
		private readonly ITaskInfoService    infoService;
		private readonly ITaskUploadService  uploadService;
		private readonly IPhotoLoader          photoLoader;

		private readonly HashSet<string> files
			= new HashSet<string>();

		private readonly HashSet<string> folders	
			= new HashSet<string>();

		private readonly Dictionary<string, int> uploadedFileCounts
			= new Dictionary<string,int>();

		#endregion

		#region interface

		public URModel
			( ITaskControlService controlService
			, ITaskInfoService    infoService
			, ITaskUploadService  uploadService
			, IPhotoLoader          photoLoader
			)
		{
			this.controlService = controlService;
			this.infoService    = infoService;
			this.uploadService  = uploadService;
			this.photoLoader    = photoLoader;

			this.infoService.TaskCancelled  += OnTaskCancelled;
			this.infoService.FoldersUpdated += OnFoldersUpdated;
		}

		#endregion

		#region IUploadResampledModel Members

		public string[] Folders { get; private set; }

		public void FetchFiles(string folderPath)
		{
			AddFiles(controlService.GetFiles(folderPath));
		}

		public void FetchFolders()
		{
			AddFolders(controlService.GetFolders());
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

		public void StartPolling()
		{
			infoService.StartPolling((int)Math.Floor(Settings.Default.InfoPollingRate.TotalMilliseconds));
		}

		public void UploadFile(string folderPath, string filePath)
		{
			IncrementUploadedFileCount(folderPath);

			if (files.Contains(filePath.ToUpperInvariant()))
				return;

			uploadService.UploadFile(folderPath, filePath, () => photoLoader.GetPreview(filePath));
		}

		public event Action<string> FolderAdded = delegate {};

		public event Action<string> FolderUploadComplete = delegate {};

		public event Action TaskCancelled
		{
			add    { infoService.TaskCancelled += value; }
			remove { infoService.TaskCancelled -= value; }
		}

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

		#endregion

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

		private void OnFoldersUpdated()
		{
			AddFolders(controlService.GetFolders());
		}

		private void OnTaskCancelled()
		{
			infoService.StopPolling();
		}

		#endregion
	}
}
