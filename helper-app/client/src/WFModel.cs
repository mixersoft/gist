using System;
using System.Collections.Generic;
using Snaphappi.Properties;

namespace Snaphappi
{
	public class WFModel : IWFModel
	{
		#region data
		
		private readonly IURTaskControlService controlService;
		private readonly IURTaskUploadService  uploadService;
		private readonly IPhotoLoader          photoLoader;

		private readonly HashSet<string> files
			= new HashSet<string>();

		private readonly HashSet<string> folders	
			= new HashSet<string>();

		#endregion // data

		public WFModel
			( IURTaskControlService controlService
			, IURTaskUploadService  uploadService
			, IPhotoLoader          photoLoader
			)
		{
			this.controlService = controlService;
			this.uploadService  = uploadService;
			this.photoLoader    = photoLoader;
		}

		#region IWFModel Members

		public void FetchFiles(string folderPath)
		{
			AddFiles(controlService.GetFiles(folderPath));
		}

		public void FetchFolders()
		{
			var folders = controlService.GetFolders();
			if (folders.Length == 0)
				FolderListEmpty();
			else
				AddFolders(folders);
		}

		public void UploadFile(string folderPath, string filePath)
		{
			if (files.Contains(filePath.ToUpperInvariant()))
				return;
			uploadService.UploadFile(folderPath, filePath, () => photoLoader.GetPreview(filePath));
		}

		public void UnscheduleWatcher()
		{
			SystemScheduler.UnscheduleWatcher();
		}

		public event Action FolderListEmpty = delegate {};

		public event Action<string> FolderAdded = delegate {};

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

		#endregion // implementation
	}
}
