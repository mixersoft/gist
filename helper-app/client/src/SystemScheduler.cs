using Microsoft.Win32.TaskScheduler;
using System.IO;
using System.Linq;
using Snaphappi.Properties;
using System;

namespace Snaphappi
{
	public class SystemScheduler
	{
		private const string folderName = "Snaphappi";

		public static void ScheduleWatcher(string authToken)
		{
			using (var taskService = new TaskService())
			{
				var timeTrigger = new TimeTrigger();
				timeTrigger.StartBoundary = DateTime.Now + TimeSpan.FromMinutes(1.0);
				timeTrigger.Repetition.Interval = Settings.Default.WatchedFolderTaskRepetitionRate;

				var execAction = new ExecAction(typeof(HelperApp).Assembly.Location, "-watch " + authToken);

				var definition = taskService.NewTask();
				definition.Triggers.Add(timeTrigger);
				definition.Actions.Add(execAction);

				taskService.RootFolder.RegisterTaskDefinition(TaskPath, definition);
			}
		}

		public static void UnscheduleWatcher()
		{
			try
			{
				using (var taskService = new TaskService())
					taskService.RootFolder.DeleteTask(TaskPath);
			}
			catch (FileNotFoundException)
			{
				// no task - no problem
			}
		}

		private static string TaskPath
		{
			get { return Path.Combine(folderName, Settings.Default.WatchedFolderTaskName); }
		}
	}
}
