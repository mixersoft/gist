using System;
using System.Collections.Generic;
using System.IO;
using Snaphappi.Properties;

namespace Snaphappi
{
	public class UOModel : IUOModel
	{
		#region data

		private readonly ITaskControlService controlService;
		private readonly ITaskInfoService    infoService;
		private readonly ITaskUploadService  uploadService;

		private readonly HashSet<string> files
			= new HashSet<string>();

		#endregion // data

		#region interface

		public UOModel
			( ITaskControlService controlService
			, ITaskInfoService    infoService
			, ITaskUploadService  uploadService
			)
		{
			this.controlService = controlService;
			this.infoService    = infoService;
			this.uploadService  = uploadService;

			this.infoService.FilesUpdated  += OnFilesUpdated;
			this.infoService.TaskCancelled += OnTaskCancelled;
		}

		#endregion // interface

		#region IUOModel Members

		public event Action<string, string> FileAdded = delegate {};

		public event Action TaskCancelled
		{
			add    { infoService.TaskCancelled += value; }
			remove { infoService.TaskCancelled -= value; }
		}

		public event Action<string, string> FileNotFound
		{
			add    { uploadService.FileNotFound += value; }
			remove { uploadService.FileNotFound -= value; }
		}

		public event Action<string, string> UploadFailed
		{
			add    { uploadService.UploadFailed += value; }
			remove { uploadService.UploadFailed -= value; }
		}

		public void UploadFile(string folderPath, string filePath)
		{
			uploadService.UploadFile(folderPath, filePath, () => File.ReadAllBytes(filePath));
		}

		public void StartPolling()
		{
			var pollingRate = Settings.Default.InfoPollingRate.TotalMilliseconds;
			infoService.StartPolling((int)Math.Floor(pollingRate));
		}

		public void FetchFiles()
		{
			AddFiles(controlService.GetFilesToUpload());
		}

		#endregion // IUOModel Members

		#region implementation

		private void AddFiles(string[] files)
		{
			foreach (var filePath in files)
			{
				var ucFilePath = filePath.ToUpperInvariant();
				if (!this.files.Contains(ucFilePath))
				{
					this.files.Add(ucFilePath);
					FileAdded("", filePath);
				}
			}
		}

		private void OnFilesUpdated()
		{
			AddFiles(controlService.GetFilesToUpload());
		}

		private void OnTaskCancelled()
		{
			infoService.StopPolling();
		}

		#endregion // implementation
	}
}
