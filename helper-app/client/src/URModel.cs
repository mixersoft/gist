using System;
using System.Collections.Generic;
using System.Threading;

namespace Snaphappi
{
	public class URModel : IURModel
	{
		#region data

		private readonly IURTaskService   taskService;
		private readonly IURUploadService uploadService;

		#endregion

		#region interface

		public URModel
			( IURTaskService   taskService
			, IURUploadService uploadService
			)
		{
			this.taskService   = taskService;
			this.uploadService = uploadService;

			this.taskService.TaskCancelled += OnTaskCancelled;
		}

		#endregion

		#region IUploadResampledModel Members

		public string[] Folders { get; private set; }

		public void DownloadInformation()
		{
			Folders = taskService.GetFolders();
			taskService.StartPolling(1000);
		}

		public void UploadFile(string filePath)
		{
			uploadService.UploadFile(filePath);
		}

		public event Action TaskCancelled
		{
			add    { taskService.TaskCancelled += value; }
			remove { taskService.TaskCancelled -= value; }
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
			taskService.StopPolling();
		}

		#endregion
	}
}
