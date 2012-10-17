using System;
using System.IO;
using System.Runtime.InteropServices;
using Snaphappi.Properties;

namespace Snaphappi
{
	class HelperApp
	{
		#region settings

		private const int ExitSuccess = 0;
		private const int ExitFailure = 1;

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
					if (!IsUnique(info))
						return ExitSuccess;
					switch (info.Type)
					{
						case ParameterProcessor.TaskType.UploadOriginals:
							UploadOriginals(info.AuthToken, info.SessionID);
							break;
						case ParameterProcessor.TaskType.UploadResampled:
							UploadResampled(info.AuthToken, info.SessionID);
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

		/// <summary>
		/// Check the command line of every other running instance to see whether it has the same task ID.
		/// </summary>
		private static bool IsUnique(ParameterProcessor.ParameterInfo info)
		{
			foreach (var args in Wmi.GetOtherCommandLines())
			{
				if (args.Length != 2)
					continue;
				var arg = args[1];
				switch (arg)
				{
					case "-ur": break;
					case "-uo": break;
					default:
						try
						{
							if (ParameterProcessor.SplitUrl(arg).Equals(info))
								return false;
						}
						catch (FormatException)
						{
							// if we can't parse the args, then we don't care about them
						}
						break;
				}
			}
			return true;
		}

		private static void UploadResampled(string authToken, string sessionID)
		{
			var app = new App();

			var controlService = new URTaskControlService (authToken, sessionID, Settings.Default.TaskURI);
			var infoService    = new URTaskInfoService    (authToken, sessionID, Settings.Default.TaskURI);
			var uploadService  = new URTaskUploadService  (authToken, sessionID, Settings.Default.TaskURI);

			var photoLoader = new PhotoLoader();

			var urModel = new URModel (controlService, infoService, uploadService, photoLoader);
			var urView  = new URView  (controlService);

			var fileSystem = new FileSystem();
			var fileLister = new FileLister(fileSystem, Settings.Default.PhotoExtensions);

			new URPresenter(app, urModel, urView, fileLister);

			app.LoadUR();
		}

		private static void UploadOriginals(string authToken, string sessionID)
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
