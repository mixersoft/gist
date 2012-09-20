using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public class UOPresenter
	{
		private readonly IApp        app;
		private readonly IUOModel    uoModel;
		private readonly IUOView     uoView;
		private readonly IFileFinder fileFinder;

		public UOPresenter
			( IApp                  app
			, IUOModel uoModel
			, IUOView  uoView
			, IFileFinder           fileFinder
			)
		{
			this.app        = app;
			this.uoModel    = uoModel;
			this.uoView     = uoView;
			this.fileFinder = fileFinder;

			app.LoadUploadOriginals += OnLoad;

			uoModel.InfoDownloaded += OnInfoDownloaded;
			uoModel.TaskCancelled  += OnTaskCancelled;
			uoModel.UploadFailed   += OnUploadFailed;

			fileFinder.FileFound    += OnFileFound;
			fileFinder.FileNotFound += OnFileNotFound;
		}

		private void OnLoad()
		{
			uoModel.DownloadInformation();
		}

		private void OnInfoDownloaded()
		{
			fileFinder.SetFiles(uoModel.FileInfo);
			fileFinder.Start();
		}

		private void OnTaskCancelled()
		{
			fileFinder.Stop();
			app.Quit();
		}

		private void OnFileFound(OriginalFileInfo file)
		{
			uoModel.UploadFile(file);
		}

		private void OnFileNotFound(OriginalFileInfo file)
		{
			uoView.ReportFileNotFound(file);
		}

		private void OnUploadFailed(OriginalFileInfo file)
		{
			uoView.ReportUploadFailed(file);
		}
	}
}
