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

		public UOPresenter
			( IApp     app
			, IUOModel uoModel
			, IUOView  uoView
			)
		{
			this.app        = app;
			this.uoModel    = uoModel;
			this.uoView     = uoView;

			app.Loaded += OnLoaded;

			uoModel.TargetAdded    += OnTargetAdded;
			uoModel.FileFound      += OnFileFound;
			uoModel.FileNotFound   += OnFileNotFound;
			uoModel.TaskCancelled  += OnTaskCancelled;
			uoModel.UploadFailed   += OnUploadFailed;
		}

		private void OnTargetAdded(UploadTarget uploadTarget)
		{
			uoModel.FindFile(uploadTarget.FilePath, uploadTarget.Hash);
		}

		private void OnFileFound(string folderPath, string filePath)
		{
			uoModel.UploadFile(folderPath, filePath);
		}

		private void OnFileNotFound(string folderPath, string filePath)
		{
			uoView.ReportFileNotFound(folderPath, filePath);
		}

		private void OnLoaded()
		{
			uoModel.StartPolling();
			uoModel.FetchFiles();
		}

		private void OnTaskCancelled()
		{
			uoModel.Stop();
			app.Quit();
		}

		private void OnUploadFailed(string folderPath, string filePath)
		{
			uoView.ReportUploadFailed(folderPath, filePath);
		}
	}
}
