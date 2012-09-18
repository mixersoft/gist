using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public class UploadOriginalsPresenter
	{
		private readonly IApp                  app;
		private readonly IUploadOriginalsModel uploadOriginalsModel;
		private readonly IUploadOriginalsView  uploadOriginalsView;
		private readonly IFileFinder           fileFinder;

		public UploadOriginalsPresenter
			( IApp                  app
			, IUploadOriginalsModel uploadOriginalsModel
			, IUploadOriginalsView  uploadOriginalsView
			, IFileFinder           fileFinder
			)
		{
			this.app                  = app;
			this.uploadOriginalsModel = uploadOriginalsModel;
			this.uploadOriginalsView  = uploadOriginalsView;
			this.fileFinder           = fileFinder;

			app.LoadUploadOriginals += OnLoad;

			uploadOriginalsModel.InfoDownloaded += OnInfoDownloaded;
			uploadOriginalsModel.TaskCancelled  += OnTaskCancelled;
			uploadOriginalsModel.UploadFailed   += OnUploadFailed;

			fileFinder.FileFound    += OnFileFound;
			fileFinder.FileNotFound += OnFileNotFound;
		}

		private void OnLoad()
		{
			uploadOriginalsModel.DownloadInformation();
		}

		private void OnInfoDownloaded()
		{
			fileFinder.SetFiles(uploadOriginalsModel.FileInfo);
			fileFinder.Start();
		}

		private void OnTaskCancelled()
		{
			fileFinder.Stop();
			app.Quit();
		}

		private void OnFileFound(OriginalFileInfo file)
		{
			uploadOriginalsModel.UploadFile(file);
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
