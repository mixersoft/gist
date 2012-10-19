using System;

namespace Snaphappi
{
	public class TaskID
	{
		public string AuthToken;
		public string SessionID;
		public string DeviceID;

		public TaskID(string authToken, string sessionID, string deviceID)
		{
			AuthToken = authToken;
			SessionID = sessionID;
			DeviceID  = deviceID;
		}
	}
}
