using System;

namespace Snaphappi
{
	class SnaphappiHelper
	{
		private const int ExitSuccess = 0;
		private const int ExitFailure = 1;

		public static int Main(string[] args)
		{
			if (args.Length != 1)
				return ExitFailure;
			try
			{
				var info = ParameterProcessor.SplitUrl(args[0]);
				switch (info.Type)
				{
					case ParameterProcessor.TaskType.UploadOriginals:
						UploadOriginals(info.TaskID, info.SessionID);
						break;
					case ParameterProcessor.TaskType.UploadResampled:
						UploadResampled(info.TaskID, info.SessionID);
						break;
				}
				return ExitSuccess;
			}
			catch (Exception)
			{
				return ExitFailure;
			}
		}

		private static void UploadResampled(int taskID, string sessionID)
		{
			var app = new App();

			var taskService   = new URTaskService(taskID, sessionID);
			var uploadService = new URUploadService(taskID, sessionID);

			var urModel = new URModel(taskService, uploadService);
			var urView  = new URView();

			var fileSystem = new FileSystem();
			var fileLister = new FileLister(fileSystem);

			new URPresenter(app, urModel, urView, fileLister);

			app.LoadUR();
		}

		private static void UploadOriginals(int p, string p_2)
		{
			throw new NotImplementedException();
		}
	}
}
