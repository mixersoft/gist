using System;

namespace Snaphappi
{
	public class ApiHelper
	{
		public static Snaphappi.API.TaskID ConvertTaskID(TaskID taskID)
		{
			var id = new Snaphappi.API.TaskID();
			id.AuthToken = taskID.AuthToken;
			id.Session   = taskID.SessionID;
			id.DeviceID  = taskID.DeviceID;
			id.__isset.AuthToken = true;
			id.__isset.Session   = true;
			id.__isset.DeviceID  = true;
			return id;
		}
	}
}
