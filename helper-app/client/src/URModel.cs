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
		
		private readonly IURTaskControlService controlService;
		private readonly IURTaskInfoService    infoService;
		private readonly IURTaskUploadService  uploadService;
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
			( IURTaskControlService controlService
			, IURTaskInfoService    infoService
			, IURTaskUploadService  uploadService
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

		public void DownloadInformation()
		{
			AddFolders(controlService.GetFolders());
			foreach (var folder in folders)
				AddFiles(controlService.GetFiles(folder));
			infoService.StartPolling((int)Math.Floor(Settings.Default.InfoPollingRate.TotalMilliseconds));
		}

		public int GetFileCount(string folderPath)
		{
			int count = 0;
			uploadedFileCounts.TryGetValue(folderPath, out count);
			return count;
		}

		public void UploadFile(string folderPath, string filePath)
		{
			if (files.Contains(filePath))
				return;

			IncrementUploadedFileCount(folderPath);

			uploadService.UploadFile(folderPath, filePath, () => photoLoader.GetPreview(filePath));
		}

		public event Action<string> FolderAdded;

		public event Action TaskCancelled
		{
			add    { infoService.TaskCancelled += value; }
			remove { infoService.TaskCancelled -= value; }
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
				if (!this.files.Contains(filePath))
					this.files.Add(filePath);
			}
		}

		private void AddFolders(string[] folders)
		{
			foreach (var folderPath in folders)
			{
				if (!this.folders.Contains(folderPath))
				{
					this.folders.Add(folderPath);
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
