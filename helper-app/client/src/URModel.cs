using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Snaphappi
{
	public class URModel : IURModel
	{
		#region data
		
		private readonly IURTaskControlService controlService;
		private readonly IURTaskInfoService   infoService;
		private readonly IURTaskUploadService uploadService;

		private readonly HashSet<string> folders = new HashSet<string>();

		#endregion

		#region interface

		public URModel
			( IURTaskControlService controlService
			, IURTaskInfoService    infoService
			, IURTaskUploadService  uploadService
			)
		{
			this.controlService = controlService;
			this.infoService    = infoService;
			this.uploadService  = uploadService;

			this.infoService.TaskCancelled += OnTaskCancelled;
		}

		#endregion

		#region IUploadResampledModel Members

		public string[] Folders { get; private set; }

		public void DownloadInformation()
		{
			AddFolders(controlService.GetFolders());
			infoService.StartPolling(1000);
		}

		public void UploadFile(string folderPath, string filePath)
		{
			uploadService.UploadFile(folderPath, filePath, () => File.ReadAllBytes(filePath));
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

		private void OnTaskCancelled()
		{
			infoService.StopPolling();
		}

		#endregion
	}
}
