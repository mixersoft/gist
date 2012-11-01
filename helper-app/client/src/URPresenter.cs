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
			( IApp        app
			, IURModel    urModel
			, IURView     urView
			, IFileLister fileLister
			)
		{
			this.app        = app;
			this.urModel    = urModel;
			this.urView     = urView;
			this.fileLister = fileLister;

			app.Loaded += OnLoaded;
			
			urModel.DuplicateUpload      += OnDuplicateUpload;
			urModel.FolderAdded          += OnFolderAdded;
			urModel.TaskCancelled        += OnTaskCancelled;
			urModel.UploadFailed         += OnUploadFailed;
			urModel.FolderUploadComplete += OnFolderUploadComplete;

			fileLister.FileFound            += OnFileFound;
			fileLister.FolderNotFound       += OnFolderNotFound;
			fileLister.FolderSearchComplete += OnFolderSearchComplete;
		}

		private void OnDuplicateUpload(string folderPath, string filePath)
		{
			urModel.FetchFiles(folderPath);
		}

		private void OnFileFound(string folderPath, string filePath)
		{
			urModel.UploadFile(folderPath, filePath);
		}

		private void OnFolderAdded(string folder)
		{
			urModel.FetchFiles(folder);
			fileLister.SearchFolder(folder);
		}

		private void OnFolderNotFound(string folderPath)
		{
			urView.ReportFolderNotFound(folderPath);
		}

		private void OnFolderSearchComplete(string folderPath)
		{
			urView.ReportFileCount(folderPath, urModel.GetFileCount(folderPath));
			urModel.ScheduleFolderUploadCompletionEvent(folderPath);
		}

		private void OnFolderUploadComplete(string folderPath)
		{
			urView.ReportFolderUploadComplete(folderPath);
		}

		private void OnLoaded()
		{
			urModel.FetchFolders();
		}

		private void OnTaskCancelled()
		{
			app.Quit();
		}
		
		private void OnUploadFailed(string folderPath, string filePath)
		{
			urView.ReportUploadFailed(folderPath, filePath);
		}
	}
}
