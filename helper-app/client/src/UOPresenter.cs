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

			app.Loaded     += OnLoaded;
			app.Terminated += OnTerminated;

			uoModel.TargetAdded        += OnTargetAdded;
			uoModel.FileFound          += OnFileFound;
			uoModel.FileNotFoundByHash += OnFileNotFoundByHash;
			uoModel.FileNotFoundByName += OnFileNotFoundByName;
			uoModel.TaskCancelled      += OnTaskCancelled;
			uoModel.UploadFailed       += OnUploadFailed;
		}

		private void OnTargetAdded(UploadTarget target)
		{
			uoModel.FindFileByName(target);
		}

		private void OnFileFound(FileMatch match)
		{
			uoModel.UploadFile(match);
		}

		private void OnFileNotFoundByHash(UploadTarget target)
		{
			uoView.ReportFileNotFound(target.ImageID);
		}

		private void OnFileNotFoundByName(UploadTarget target)
		{
			uoModel.FindFileByHash(target, uoModel.GetImageHash(target.ImageID));
		}

		private void OnLoaded()
		{
			uoModel.StartPolling();
			uoModel.FetchFiles();
		}

		private void OnTaskCancelled()
		{
			app.Quit();
		}

		private void OnTerminated()
		{
			uoModel.Stop();
		}

		private void OnUploadFailed(UploadTarget target)
		{
			uoView.ReportUploadFailed(target.ImageID);
		}
	}
}
