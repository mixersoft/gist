using System;

namespace Snaphappi
{
	public class WFPresenter
	{
		private readonly IApp        app;
		private readonly IWFModel    wfModel;
		private readonly IFileLister fileLister;

		public WFPresenter
			( IApp       app
			, IWFModel    wfModel
			, FileLister fileLister
			)
		{
			this.app        = app;
			this.wfModel    = wfModel;
			this.fileLister = fileLister;

			app.Loaded += OnLoaded;

			wfModel.FolderListEmpty      += OnFolderListEmpty;
			wfModel.FolderAdded          += OnFolderAdded;
			wfModel.FolderUploadComplete += OnFolderUploadComplete;

			fileLister.FileFound            += OnFileFound;
			fileLister.FolderSearchComplete += OnFolderSearchComplete;
		}

		private void OnLoaded()
		{
			wfModel.FetchFolders();
		}

		private void OnFolderListEmpty()
		{
			wfModel.UnscheduleWatcher();
			app.Quit();
		}

		private void OnFolderAdded(string folderPath)
		{
			wfModel.FetchFiles(folderPath);
			fileLister.SearchFolder(folderPath);
		}

		private void OnFolderSearchComplete(string folderPath)
		{
			wfModel.ScheduleFolderUploadCompletionEvent(folderPath);
		}

		private void OnFolderUploadComplete(string folderPath)
		{
			app.Quit();
		}

		private void OnFileFound(string folderPath, string filePath)
		{
			wfModel.UploadFile(folderPath, filePath);
		}
	}
}
