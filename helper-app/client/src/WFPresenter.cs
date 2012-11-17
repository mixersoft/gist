using System;

namespace Snaphappi
{
	public class WFPresenter
	{
		private readonly IApp        app;
		private readonly IWFModel    wfModel;
		private readonly IWFView     wfView;
		private readonly IFileLister fileLister;

		public WFPresenter
			( IApp       app
			, IWFModel   wfModel
			, IWFView    wfView
			, FileLister fileLister
			)
		{
			this.app        = app;
			this.wfModel    = wfModel;
			this.wfView     = wfView;
			this.fileLister = fileLister;

			app.Loaded += OnLoaded;

			wfModel.AuthTokenRejected    += OnAuthTokenRejected;
			wfModel.DuplicateUpload      += OnDuplicateUpload;
			wfModel.FolderListEmpty      += OnFolderListEmpty;
			wfModel.FolderAdded          += OnFolderAdded;
			wfModel.FolderUploadComplete += OnFolderUploadComplete;
			wfModel.UploadFailed         += OnUploadFailed;

			fileLister.FileFound            += OnFileFound;
			fileLister.FolderSearchComplete += OnFolderSearchComplete;
			fileLister.FolderNotFound       += OnFolderNotFound;
		}

		private void OnAuthTokenRejected()
		{
			wfModel.UnscheduleWatcher();
			app.Quit();
		}

		private void OnDuplicateUpload(string folderPath, string filePath)
		{
			wfModel.FetchFiles(folderPath);
		}

		private void OnFileFound(string folderPath, string filePath)
		{
			wfModel.UploadFile(folderPath, filePath);
		}

		private void OnFolderAdded(string folderPath)
		{
			wfModel.FetchFiles(folderPath);
			fileLister.SearchFolder(folderPath);
		}

		private void OnFolderListEmpty()
		{
			wfModel.UnscheduleWatcher();
			app.Quit();
		}

		private void OnFolderNotFound(string folderPath)
		{
			wfView.ReportFolderNotFound(folderPath);
		}

		private void OnFolderSearchComplete(string folderPath)
		{
			wfView.ReportFileCount(folderPath, wfModel.GetFileCount(folderPath));
			wfModel.ScheduleFolderUploadCompletionEvent(folderPath);
		}

		private void OnFolderUploadComplete(string folderPath)
		{
			wfView.ReportFolderUploadComplete(folderPath);
			app.Quit();
		}

		private void OnLoaded()
		{
			wfModel.FetchFolders();
		}

		private void OnUploadFailed(string folderPath, string filePath)
		{
			wfView.ReportUploadFailed(folderPath, filePath);
		}
	}
}
