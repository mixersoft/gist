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
			Folders = controlService.GetFolders();
			infoService.StartPolling(1000);
		}

		public void UploadFile(string filePath)
		{
			uploadService.UploadFile(filePath, () => File.ReadAllBytes(filePath));
		}

		public event Action TaskCancelled
		{
			add    { infoService.TaskCancelled += value; }
			remove { infoService.TaskCancelled -= value; }
		}

		public event Action<string> UploadFailed
		{
			add    { uploadService.UploadFailed += value; }
			remove { uploadService.UploadFailed -= value; }
		}

		#endregion

		#region implementation

		private void OnTaskCancelled()
		{
			infoService.StopPolling();
		}

		#endregion
	}
}
