using System;

namespace Snaphappi
{
	public class TaskID
	{
		public readonly string AuthToken;
		public readonly string SessionID;
		public readonly string DeviceID;

		public TaskID(string authToken, string sessionID, string deviceID)
		{
			AuthToken = authToken;
			SessionID = sessionID;
			DeviceID  = deviceID;
		}
	}
}
