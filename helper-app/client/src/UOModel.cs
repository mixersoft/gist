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

			this.fileFinder.FileFound    += OnFileFound;
			this.fileFinder.FileNotFound += OnFileNotFound;

			this.infoService.FilesUpdated  += OnFilesUpdated;

			// same handling for all upload failures
			this.uploadService.FileNotFound += OnUploadFailed;
			this.uploadService.UploadFailed += OnUploadFailed;
		}

		#endregion // interface

		#region IUOModel Members
		
		public event Action<FileMatch> FileFound        = delegate {};
		public event Action<UploadTarget> FileNotFound  = delegate {};
		public event Action<UploadTarget> TargetAdded   = delegate {};
		public event Action<UploadTarget> UploadFailed  = delegate {};

		public event Action TaskCancelled
		{
			add    { infoService.TaskCancelled += value; }
			remove { infoService.TaskCancelled -= value; }
		}

		public void FetchFiles()
		{
			AddFiles(controlService.GetFilesToUpload());
		}

		public void FindFile(UploadTarget target)
		{
			fileFinder.Find(target.FilePath, target.Timestamp, target.Hash);
		}

		public void StartPolling()
		{
			var pollingRate = Settings.Default.InfoPollingRate.TotalMilliseconds;
			infoService.StartPolling((int)Math.Floor(pollingRate));
		}

		public void Stop()
		{
			fileFinder.Stop();
			infoService.StopPolling();
			uploadService.Stop();
		}

		public void UploadFile(FileMatch match)
		{
			uploadService.UploadFile
				( GetUploadTarget(match.OldLocation).FolderPath
				, match.OldLocation
				, match.NewLocation
				, UploadType.Original
				, () => File.ReadAllBytes(match.OldLocation)
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

		private UploadTarget GetUploadTarget(string filePath)
		{
			return uploadTargets[filePath.ToUpperInvariant()];
		}

		private void OnFileFound(FileMatch match)
		{
			FileFound(match);
		}

		private void OnFileNotFound(string filePath)
		{
			FileNotFound(GetUploadTarget(filePath));
		}

		private void OnUploadFailed(string folderPath, string filePath)
		{
			UploadFailed(GetUploadTarget(filePath));
		}

		private void OnFilesUpdated()
		{
			AddFiles(controlService.GetFilesToUpload());
		}

		#endregion // implementation
	}
}
