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
			
			urModel.FolderAdded    += OnFolderAdded;
			urModel.TaskCancelled  += OnTaskCancelled;
			urModel.UploadFailed   += OnUploadFailed;

			fileLister.FileFound            += OnFileFound;
			fileLister.FolderNotFound       += OnFolderNotFound;
			fileLister.FolderSearchComplete += OnFolderSearchComplete;
		}

		private void OnLoad()
		{
			urModel.DownloadInformation();
		}

		private void OnFolderAdded(string folder)
		{
			fileLister.SearchFolder(folder);
		}

		private void OnTaskCancelled()
		{
			app.Quit();
		}

		private void OnFileFound(string folderPath, string filePath)
		{
			urModel.UploadFile(folderPath, filePath);
		}
		
		private void OnUploadFailed(string folderPath, string filePath)
		{
			urView.ReportUploadFailed(folderPath, filePath);
		}

		private void OnFolderNotFound(string folderPath)
		{
			urView.ReportFolderNotFound(folderPath);
		}

		private void OnFolderSearchComplete(string folderPath)
		{
			urView.ReportFileCount(folderPath, urModel.GetFileCount(folderPath));
		}
	}
}
