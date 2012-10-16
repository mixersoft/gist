using System;
using System.Collections.Generic;
using System.Text;
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
			public readonly string   AuthToken;
			public readonly string   SessionID;
			public readonly TaskType Type;

			public ParameterInfo(string authToken, string session, TaskType type)
			{
				this.AuthToken = authToken;
				this.SessionID = session;
				this.Type      = type;
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
				var match = Regex.Match(url, @"snaphappi://(.+)_(.+)_([a-z]+)");
				return new ParameterInfo
					( DecodeString(match.Groups[1].Value)
					, DecodeString(match.Groups[2].Value)
					, taskTypeMap[match.Groups[3].Value]
					);
			}
			catch (Exception e)
			{
				throw new FormatException(url, e);
			}
		}

		private static string DecodeString(string str)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(str));
		}
	}
}
