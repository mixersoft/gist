using System;

namespace Snaphappi
{
	class URPresenter
	{
		private readonly IApp        app;
		private readonly IURModel    urModel;
		private readonly IURView     urView;
		private readonly IFileLister fileLister;

		public URPresenter
			( IApp app
			, IURModel urModel
			, IURView  urView
			, IFileLister           fileLister
			)
		{
			this.app        = app;
			this.urModel    = urModel;
			this.urView     = urView;
			this.fileLister = fileLister;

			app.LoadUploadResampled += OnLoad;

			urModel.TaskCancelled  += OnTaskCancelled;
			urModel.UploadFailed   += OnUploadFailed;

			fileLister.FileFound += OnFileFound;
			fileLister.FolderNotFound += OnFolderNotFound;
		}

		private void OnLoad()
		{
			urModel.DownloadInformation();
			fileLister.UpdateFolders(urModel.Folders);
			fileLister.Start();
		}

		private void OnTaskCancelled()
		{
			fileLister.Stop();
			app.Quit();
		}

		private void OnFileFound(string file)
		{
			urModel.UploadFile(file);
		}
		
		private void OnUploadFailed(string file)
		{
			urView.ReportUploadFailed(file);
		}

		private void OnFolderNotFound(string path)
		{
			urView.ReportFolderNotFound(path);
		}
	}
}
