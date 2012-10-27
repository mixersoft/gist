using Microsoft.Win32.TaskScheduler;
using Snaphappi.Properties;
using System;

namespace Snaphappi
{
	public class SystemScheduler
	{

		public static void SetWatcher(string authToken)
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

				taskService.RootFolder.RegisterTaskDefinition
					( @"Snaphappi\" + Settings.Default.WatchedFolderTaskName
					, definition
					);
			}
		}
	}
}
