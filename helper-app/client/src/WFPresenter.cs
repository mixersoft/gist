﻿using System;

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

			wfModel.FolderListEmpty      += OnFolderListEmpty;
			wfModel.FolderAdded          += OnFolderAdded;
			wfModel.FolderUploadComplete += OnFolderUploadComplete;
			wfModel.UploadFailed         += OnUploadFailed;

			fileLister.FileFound            += OnFileFound;
			fileLister.FolderSearchComplete += OnFolderSearchComplete;
			fileLister.FolderNotFound       += OnFolderNotFound;
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
			wfModel.ScheduleFolderUploadCompletionEvent(folderPath);
		}

		private void OnFolderUploadComplete(string folderPath)
		{
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
