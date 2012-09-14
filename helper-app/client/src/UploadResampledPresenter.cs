using System;

namespace Snaphappi
{
	class UploadResampledPresenter
	{
		private readonly IApp                  app;
		private readonly IUploadResampledModel uploadResampledModel;
		private readonly IUploadResampledView  uploadResampledView;
		private readonly IFileFinder           fileFinder;

		public UploadResampledPresenter
			( IApp app
			, IUploadResampledModel uploadResampledModel
			, IUploadResampledView  uploadResampledView
			, IFileFinder           fileFinder
			)
		{
			this.app                  = app;
			this.uploadResampledModel = uploadResampledModel;
			this.uploadResampledView  = uploadResampledView;
			this.fileFinder           = fileFinder;

			app.LoadUploadResampled += OnLoad;

			uploadResampledModel.InfoDownloaded += OnInfoDownloaded;
			uploadResampledModel.TaskCancelled  += OnTaskCancelled;
			uploadResampledModel.UploadFailed   += OnUploadFailed;

			fileFinder.FileFound    += OnFileFound;
			fileFinder.FileNotFound += OnFileNotFound;
		}

		private void OnLoad()
		{
			uploadResampledModel.DownloadInformation();
		}

		private void OnInfoDownloaded()
		{
			fileFinder.FileInfo = uploadResampledModel.FileInfo;
			fileFinder.Start();
		}

		private void OnTaskCancelled()
		{
			fileFinder.Stop();
			app.Quit();
		}

		private void OnFileFound(OriginalFileInfo file)
		{
			uploadResampledModel.UploadFile(file);
		}

		private void OnFileNotFound(OriginalFileInfo file)
		{
			uploadResampledView.ReportFileNotFound(file);
		}

		private void OnUploadFailed(OriginalFileInfo file)
		{
			uploadResampledView.ReportUploadFailed(file);
		}
	}
}
