using System;

namespace Snaphappi
{
	class UploadResampledPresenter
	{
		private readonly IApp                  app;
		private readonly IUploadResampledModel uploadResampledModel;
		private readonly IUploadResampledView  uploadResampledView;
		private readonly IFileLister           fileLister;

		public UploadResampledPresenter
			( IApp app
			, IUploadResampledModel uploadResampledModel
			, IUploadResampledView  uploadResampledView
			, IFileLister           fileLister
			)
		{
			this.app                  = app;
			this.uploadResampledModel = uploadResampledModel;
			this.uploadResampledView  = uploadResampledView;
			this.fileLister           = fileLister;

			app.LoadUploadResampled += OnLoad;

			uploadResampledModel.InfoDownloaded += OnInfoDownloaded;
			uploadResampledModel.TaskCancelled  += OnTaskCancelled;
			uploadResampledModel.UploadFailed   += OnUploadFailed;

			fileLister.FileFound += OnFileFound;
			fileLister.FolderNotFound += OnFolderNotFound;
		}

		private void OnLoad()
		{
			uploadResampledModel.DownloadInformation();
		}

		private void OnInfoDownloaded()
		{
			fileLister.UpdateFolders(uploadResampledModel.Folders);
			fileLister.Start();
		}

		private void OnTaskCancelled()
		{
			fileLister.Stop();
			app.Quit();
		}

		private void OnFileFound(string file)
		{
			uploadResampledModel.UploadFile(file);
		}
		
		private void OnUploadFailed(string file)
		{
			uploadResampledView.ReportUploadFailed(file);
		}

		private void OnFolderNotFound(string path)
		{
			uploadResampledView.ReportFolderNotFound(path);
		}
	}
}
