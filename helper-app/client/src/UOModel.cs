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
		private readonly IAsyncFileFinder    fileFinder;

		private readonly Dictionary<string, UploadTarget> uploadTargets
			= new Dictionary<string, UploadTarget>();

		#endregion // data

		#region interface

		public UOModel
			( ITaskControlService controlService
			, ITaskInfoService    infoService
			, ITaskUploadService  uploadService
			, IAsyncFileFinder    fileFinder
			)
		{
			this.controlService = controlService;
			this.infoService    = infoService;
			this.uploadService  = uploadService;
			this.fileFinder     = fileFinder;

			this.fileFinder.FileFound += OnFileFound;

			this.infoService.FilesUpdated  += OnFilesUpdated;
			this.infoService.TaskCancelled += OnTaskCancelled;
		}

		#endregion // interface

		#region IUOModel Members

		public event Action<string, string> FileFound = delegate {};

		public event Action<UploadTarget> TargetAdded = delegate {};

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

		public void FetchFiles()
		{
			AddFiles(controlService.GetFilesToUpload());
		}

		public void FindFile(string path, int hash)
		{
			fileFinder.Find(path, hash);
		}

		public void StartPolling()
		{
			var pollingRate = Settings.Default.InfoPollingRate.TotalMilliseconds;
			infoService.StartPolling((int)Math.Floor(pollingRate));
		}

		public void Stop()
		{
			fileFinder.Stop();
		}

		public void UploadFile(string folderPath, string filePath)
		{
			uploadService.UploadFile
				( folderPath
				, filePath
				, UploadType.Original
				, () => File.ReadAllBytes(filePath)
				);
		}

		#endregion // IUOModel Members

		#region implementation

		private void AddFiles(UploadTarget[] uploadTargets)
		{
			foreach (var target in uploadTargets)
			{
				var ucFilePath = target.FilePath.ToUpperInvariant();
				if (!this.uploadTargets.ContainsKey(ucFilePath))
				{
					this.uploadTargets.Add(ucFilePath, target);
					TargetAdded(target);
				}
			}
		}

		private void OnFileFound(FileMatch match)
		{
			var target = uploadTargets[match.OldLocation];
			FileFound(target.FolderPath, target.FilePath);
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
