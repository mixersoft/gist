using Snaphappi.API;
using System;

namespace Snaphappi
{
	public class ApiHelper
	{
		public static TaskID MakeTaskID(string authToken, string sessionID)
		{
			var id = new TaskID();
			id.AuthToken = authToken;
			id.Session   = sessionID;
			id.__isset.AuthToken = true;
			id.__isset.Session   = true;
			return id;
		}
	}
}
