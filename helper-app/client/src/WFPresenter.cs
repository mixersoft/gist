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

			wfModel.FolderListEmpty += OnFolderListEmpty;
			wfModel.FolderAdded     += OnFolderAdded;

			fileLister.FileFound += OnFileFound;
		}

		private void OnLoaded()
		{
			wfModel.FetchFolders();
		}

		private void OnFolderListEmpty()
		{
			wfModel.UnscheduleWatcher();
		}

		private void OnFolderAdded(string folderPath)
		{
			wfModel.FetchFiles(folderPath);
			fileLister.SearchFolder(folderPath);
		}

		private void OnFileFound(string folderPath, string filePath)
		{
			wfModel.UploadFile(folderPath, filePath);
		}
	}
}
