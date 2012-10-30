using System;
using System.IO;
using System.Runtime.InteropServices;
using Snaphappi.Properties;
using System.Text.RegularExpressions;

namespace Snaphappi
{
	class HelperApp
	{
        // connect to server
		public static int Main(string[] args)
		{
			if (args.Length < 1)
				return App.ExitFailure;

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			// choose execution path based on the command line parameter
			switch (args[0])
			{
				case "-ur":    TestUploadResampled(); break;
				case "-uo":    TestUploadOriginals(); break;
				case "-w":     TestWatchFolders();    break;
				case "-watch": WatchFolders(args[1]); break;
				default:
					var info = ParameterProcessor.SplitUrl(args[0]);
					if (!IsUnique(info))
						return App.ExitSuccess;
					switch (info.Type)
					{
						case ParameterProcessor.TaskType.UploadOriginals:
							UploadOriginals(info.AuthToken, info.SessionID);
							break;
						case ParameterProcessor.TaskType.UploadResampled:
							UploadResampled(info.AuthToken, info.SessionID);
							break;
						case ParameterProcessor.TaskType.SetWatcher:
							SystemScheduler.ScheduleWatcher(info.AuthToken);
							break;
					}
					break;
			}
			return App.ExitSuccess;
		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Environment.Exit(App.ExitFailure);
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
					case "-w":  break;
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
		
		//------
		// tasks
		//------

		private static void UploadResampled(string authToken, string sessionID)
		{
			var app = new App();

			var registry = new Registry();
			var deviceID = new DeviceID(registry, Settings.Default.RegistryKey).GetID();

			var taskID = new TaskID(authToken, sessionID, deviceID);

			var controlService = new URTaskControlService (taskID, Settings.Default.TaskURI);
			var infoService    = new URTaskInfoService    (taskID, Settings.Default.TaskURI);
			var uploadService  = new URTaskUploadService  (taskID, Settings.Default.TaskURI);

			var photoLoader = new PhotoLoader();

			var urModel = new URModel (controlService, infoService, uploadService, photoLoader);
			var urView  = new URView  (controlService);

			var fileSystem = new FileSystem();
			var fileLister = new FileLister(fileSystem, Settings.Default.PhotoExtensions);

			new URPresenter(app, urModel, urView, fileLister);

			app.Load();
		}

		private static void UploadOriginals(string authToken, string sessionID)
		{
			throw new NotImplementedException();
		}

		private static void WatchFolders(string authToken)
		{
			var app = new App();

			var registry = new Registry();
			var deviceID = new DeviceID(registry, Settings.Default.RegistryKey).GetID();

			var taskID = new TaskID(authToken, "", deviceID);

			var controlService = new URTaskControlService (taskID, Settings.Default.TaskURI);
			var uploadService  = new URTaskUploadService  (taskID, Settings.Default.TaskURI);

			var photoLoader = new PhotoLoader();

			var wfModel = new WFModel (controlService, uploadService, photoLoader);
			var wfView  = new WFView  (controlService);

			var fileSystem = new FileSystem();
			var fileLister = new FileLister(fileSystem, Settings.Default.PhotoExtensions);

			new WFPresenter(app, wfModel, wfView, fileLister);

			app.Load();
		}
		
		//--------
		// testing
		//--------

		private static void TestUploadResampled()
		{
			ConsoleHelper.Alloc();
			ConsoleHelper.Title = "Snaphappi Helper Console";

			var server = new Server();

			var photoLoader = new PhotoLoader();

			var urModel = new URModel(server, server, server, photoLoader);
			var urView  = new URView(server);

			var fileSystem = new FileSystem();
			var fileLister = new FileLister(fileSystem, Settings.Default.PhotoExtensions);

			new URPresenter(server, urModel, urView, fileLister);
		}

		private static void TestUploadOriginals()
		{
		}

		private static void TestWatchFolders()
		{
			ConsoleHelper.Alloc();
			ConsoleHelper.Title = "Snaphappi Helper Console";

			var server = new Server();

			var photoLoader = new PhotoLoader();

			var wfModel = new WFModel (server, server, photoLoader);
			var wfView  = new WFView  (server);

			var fileSystem = new FileSystem();
			var fileLister = new FileLister(fileSystem, Settings.Default.PhotoExtensions);

			new WFPresenter(server, wfModel, wfView, fileLister);
		}
	}
}
