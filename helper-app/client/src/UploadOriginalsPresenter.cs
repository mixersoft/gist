using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	class UploadOriginalsPresenter
	{
		private IApp             appModel;
		private IUploadOriginalsModel uploadOriginalsModel;
		private IOriginalFileManager  originalFileManager;
		private IUploadOriginalsView uploadOriginalsView;

		public UploadOriginalsPresenter
			( IApp             appModel
			, IUploadOriginalsModel uploadOriginalsModel
			, IUploadOriginalsView  uploadOriginalsView
			, IOriginalFileManager  originalFileManager
			)
		{
			this.appModel             = appModel;
			this.uploadOriginalsModel = uploadOriginalsModel;
			this.uploadOriginalsView  = uploadOriginalsView;
			this.originalFileManager  = originalFileManager;

			appModel.LoadUploadOriginals += OnLoadUploadOriginals;

			uploadOriginalsModel.InfoDownloaded += OnInfoDownloaded;
			uploadOriginalsModel.FolderAdded    += OnFolderAdded;
			uploadOriginalsModel.TaskCancelled  += OnTaskCancelled;

			originalFileManager.FolderNotFound += OnFolderNotFound;
			originalFileManager.FileNotFound   += OnFileNotFound;
			originalFileManager.UploadFailed   += OnUploadFailed;
		}

		private void OnLoadUploadOriginals()
		{
			uploadOriginalsModel.DownloadInformation();
		}

		private void OnInfoDownloaded()
		{
			originalFileManager.FileInfo = uploadOriginalsModel.FileInfo;
			originalFileManager.Start();
		}

		private void OnFolderAdded()
		{
			originalFileManager.Folders = uploadOriginalsModel.Folders;
		}

		private void OnTaskCancelled()
		{
			originalFileManager.Stop();
			appModel.Quit();
		}

		private void OnFolderNotFound(string folder)
		{
			uploadOriginalsView.ReportFolderNotFound(folder);
		}

		private void OnFileNotFound(OriginalFileInfo file)
		{
			uploadOriginalsView.ReportFileNotFound(file);
		}

		private void OnUploadFailed(OriginalFileInfo file)
		{
			uploadOriginalsView.ReportUploadFailed(file);
		}
	}
}
