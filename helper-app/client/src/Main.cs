﻿using System;
using System.Runtime.InteropServices;
using Snaphappi.Properties;

namespace Snaphappi
{
	class SnaphappiHelper
	{
		#region settings

		private const int ExitSuccess = 0;
		private const int ExitFailure = 1;

		private static readonly Uri urTaskControlUri = new Uri(@"http://dev.snaphappi.com/thrift/service/api:1-0/URTaskControl");
		private static readonly Uri urTaskInfoUri    = new Uri(@"http://dev.snaphappi.com/thrift/service/api:1-0/URTaskInfo");
		private static readonly Uri urTaskUploadUri  = new Uri(@"http://dev.snaphappi.com/thrift/service/api:1-0/URTaskUpload");

		// connect to localhost
		//private static readonly Uri urTaskControlUri = new Uri(@"http://snappi-dev/thrift/service/api:1-0/URTaskControl");
		//private static readonly Uri urTaskInfoUri    = new Uri(@"http://snappi-dev/thrift/service/api:1-0/URTaskInfo");
		//private static readonly Uri urTaskUploadUri = new Uri(@"http://snappi-dev/thrift/service/api:1-0/URTaskUpload");

		#endregion

		public static int Main(string[] args)
		{
			if (args.Length != 1)
				return ExitFailure;

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

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

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Environment.Exit(ExitFailure);
		}

		private static void UploadResampled(int taskID, string sessionID)
		{
			var app = new App();

			var controlService = new URTaskControlService (taskID, sessionID, urTaskControlUri);
			var infoService    = new URTaskInfoService    (taskID, sessionID, urTaskInfoUri);
			var uploadService  = new URTaskUploadService  (taskID, sessionID, urTaskUploadUri);

			var photoLoader = new PhotoLoader();

			var urModel = new URModel (controlService, infoService, uploadService, photoLoader);
			var urView  = new URView  (controlService);

			var fileSystem = new FileSystem();
			var fileLister = new FileLister(fileSystem, Settings.Default.PhotoExtensions);

			new URPresenter(app, urModel, urView, fileLister);

			app.LoadUR();
		}

		private static void UploadOriginals(int taskID, string sessionID)
		{
			throw new NotImplementedException();
		}

		private static void TestUploadResampled()
		{
			ConsoleHelper.Alloc();
			ConsoleHelper.Title = "Snaphappi Helper Console";

			var app = new App();

			var server = new Server(Server.TaskType.UploadResampled);

			var photoLoader = new PhotoLoader();

			var urModel = new URModel(server, server, server, photoLoader);
			var urView  = new URView(server);

			var fileSystem = new FileSystem();
			var fileLister = new FileLister(fileSystem, Settings.Default.PhotoExtensions);

			new URPresenter(app, urModel, urView, fileLister);

			app.LoadUR();
		}

		private static void TestUploadOriginals()
		{
		}
	}
}
