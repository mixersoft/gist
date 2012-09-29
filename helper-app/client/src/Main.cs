using System;
using System.Runtime.InteropServices;

namespace Snaphappi
{
	class SnaphappiHelper
	{
		private const int ExitSuccess = 0;
		private const int ExitFailure = 1;

		private static readonly Uri urTaskControlUri = new Uri(@"http://dev.snaphappi.com/thrift/service/URTaskControl");
		private static readonly Uri urTaskInfoUri    = new Uri(@"http://dev.snaphappi.com/thrift/service/URTaskInfo");
		private static readonly Uri urTaskUploadUri  = new Uri(@"http://dev.snaphappi.com/thrift/service/URTaskUpload");

		public static int Main(string[] args)
		{
			if (args.Length != 1)
				return ExitFailure;
			try
			{
				// choose execution path based on the command line parameter
				switch (args[0])
				{
					case "-ur": TestUploadResampled(); break;
					case "-uo": TestUploadOriginals(); break;
					default:
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

			var controlService = new URTaskControlService (taskID, sessionID, urTaskControlUri);
			var infoService    = new URTaskInfoService    (taskID, sessionID, urTaskInfoUri);
			var uploadService  = new URTaskUploadService  (taskID, sessionID, urTaskUploadUri);

			var urModel = new URModel (controlService, infoService, uploadService);
			var urView  = new URView  (controlService);

			var fileSystem = new FileSystem();
			var fileLister = new FileLister(fileSystem);

			new URPresenter(app, urModel, urView, fileLister);

			app.LoadUR();
		}

		private static void UploadOriginals(int p, string p_2)
		{
			throw new NotImplementedException();
		}

		private static void TestUploadResampled()
		{
			ConsoleHelper.Alloc();
			ConsoleHelper.Title = "Snaphappi Helper Console";

			var app = new App();

			var server = new Server(Server.TaskType.UploadResampled);

			var urModel = new URModel(server, server, server);
			var urView  = new URView(server);

			var fileSystem = new FileSystem();
			var fileLister = new FileLister(fileSystem);

			new URPresenter(app, urModel, urView, fileLister);

			app.LoadUR();
		}

		private static void TestUploadOriginals()
		{
		}
		
	}
}
