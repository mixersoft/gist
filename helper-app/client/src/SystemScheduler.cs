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
				var task = taskService.GetTask(TaskPath);
				if (task == null)
				{
					// schedule the task to run in 1 minute, and then repeat at a set interval
					var timeTrigger = new TimeTrigger();
					timeTrigger.StartBoundary = DateTime.Now + TimeSpan.FromMinutes(1.0);
					timeTrigger.Repetition.Interval = Settings.Default.WatchedFolderTaskRepetitionRate;

					// have the task run the app with special arguments
					var action = new ExecAction(ExePath, MakeArguments(authToken));

					// register the task
					var definition = taskService.NewTask();
					definition.Triggers.Add(timeTrigger);
					definition.Actions.Add(action);
					taskService.RootFolder.RegisterTaskDefinition(TaskPath, definition);
				}
				else
				{
					var definition = task.Definition;

					// if the action does not exist, create it
					var arguments = MakeArguments(authToken);
					var action = definition.Actions.FirstOrDefault
						(a => (a is ExecAction) && ((ExecAction)a).Arguments == arguments);
					if (action == null)
						definition.Actions.Add(new ExecAction(ExePath, arguments));

					// update the task, setting it to run in 1 minute
					definition.Triggers[0].StartBoundary = DateTime.Now + TimeSpan.FromMinutes(1.0);
					taskService.RootFolder.RegisterTaskDefinition(TaskPath, definition);
				}
			}
		}

		public static void UnscheduleWatcher(string authToken)
		{
			try
			{
				using (var taskService = new TaskService())
				{
					var task = taskService.GetTask(TaskPath);
					if (task != null)
					{
						var actions = task.Definition.Actions;

						// remove all actions with our special arguments
						var arguments = "-watch " + authToken;
						for (int i = 0; i != actions.Count; ++i)
						{
							var action = actions[i];
							if (!((action is ExecAction) && ((ExecAction)action).Arguments == arguments))
								continue;
							actions.RemoveAt(i);
							--i;
						}

						// update or remove the task
						if (actions.Count > 0)
							taskService.RootFolder.RegisterTaskDefinition(TaskPath, task.Definition);
						else
							taskService.RootFolder.DeleteTask(TaskPath);
					}
				}
			}
			catch (FileNotFoundException)
			{
				// no task - no problem
			}
		}

		private static string MakeArguments(string authToken)
		{
			 return "-watch " + ParameterProcessor.EncodeWatchParameter(authToken);
		}

		private static string TaskPath
		{
			get { return Path.Combine(folderName, Settings.Default.WatchedFolderTaskName); }
		}

		private static string ExePath
		{
			get { return typeof(SystemScheduler).Assembly.Location; }
		}
	}
}
