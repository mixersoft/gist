using Snaphappi.API;
using System;

namespace Snaphappi
{
	public class ApiHelper
	{
		public static TaskID MakeTaskID(int taskID, string sessionID)
		{
			var id = new TaskID();
			id.Task    = taskID;
			id.Session = sessionID;
			id.__isset.Session = true;
			id.__isset.Task    = true;
			return id;
		}
	}
}
