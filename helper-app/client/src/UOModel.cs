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

		public event Action<FileMatch>    FileFound          = delegate {};
		public event Action<UploadTarget> FileNotFoundByHash = delegate {};
		public event Action<UploadTarget> FileNotFoundByName = delegate {};
		public event Action<UploadTarget> TargetAdded        = delegate {};
		public event Action<UploadTarget> UploadFailed       = delegate {};

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
			fileFinder.FindByName(target);
		}

		public void FindFileByHash(UploadTarget target, int hash)
		{
			fileFinder.FindByHash(target, hash);
		}

		public void FindFileByName(UploadTarget target)
		{
			fileFinder.FindByName(target);
		}

		public int GetImageHash(int imageID)
		{
			return controlService.GetImageHash(imageID);
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
				( ""
				, match.Target.ImageID
				, match.NewPath
				, UploadType.Original
				, () => File.ReadAllBytes(match.NewPath)
				);
		}

		#endregion // IUOModel Members

		#region implementation

		private void AddFiles(UploadTarget[] uploadTargets)
		{
			foreach (var target in uploadTargets)
					TargetAdded(target);
		}

		private void OnFileFound(FileMatch match)
		{
			if (uploadTargets.ContainsKey(match.NewPath))
				uploadTargets[match.NewPath] = match.Target;
			else
				uploadTargets.Add(match.NewPath, match.Target);
			FileFound(match);
		}

		private void OnFileNotFound(UploadTarget target, SearchType searchType)
		{
			switch (searchType)
			{
				case SearchType.Hash: FileNotFoundByHash(target); break;
				case SearchType.Name: FileNotFoundByName(target); break;
				default: throw new ArgumentOutOfRangeException("searchType");
			}
		}

		private void OnUploadFailed(string folderPath, string filePath)
		{
			UploadFailed(uploadTargets[filePath]);
		}

		private void OnFilesUpdated()
		{
			AddFiles(controlService.GetFilesToUpload());
		}

		#endregion // implementation
	}
}
