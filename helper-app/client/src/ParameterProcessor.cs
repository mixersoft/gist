using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Snaphappi
{
	public class ParameterProcessor
	{
		public enum TaskType
		{
			UploadResampled,
			UploadOriginals,
		}

		public class ParameterInfo
		{
			public readonly int      TaskID;
			public readonly string   SessionID;
			public readonly TaskType Type;

			public ParameterInfo(int task, string session, TaskType type)
			{
				this.TaskID    = task;
				this.SessionID = session;
				this.Type    = type;
			}
		}

		/// <summary>
		/// Split the URL passed as the command line parameter. The URL consists of the task ID,
		/// the session ID, and the task type. Example: snaphappi://5_abc_ur
		/// </summary>
		public static ParameterInfo SplitUrl(string url)
		{
			var taskTypeMap = new Dictionary<string, TaskType>();
			taskTypeMap["ur"] = TaskType.UploadResampled;
			taskTypeMap["uo"] = TaskType.UploadOriginals;

			try
			{
				var match = Regex.Match(url, @"snaphappi://(.*)_(.*)_(.*)");
				return new ParameterInfo
					( int.Parse(match.Groups[1].Value)
					, match.Groups[2].Value
					, taskTypeMap[match.Groups[3].Value]
					);
			}
			catch (Exception e)
			{
				throw new FormatException(url, e);
			}
		}
	}
}
