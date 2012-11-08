using System;
using Microsoft.Deployment.WindowsInstaller;

namespace Snaphappi
{
	public class CustomActions
	{
		[CustomAction]
		public static ActionResult InstallDeviceID(Session session)
		{
			session.Log("Begin InstallDeviceID");

			var id = new DeviceID(new Registry(), @"HKEY_LOCAL_MACHINE\Software\Snaphappi").GetID();

			session.Log("Recorded DeviceID '{0}'", id);

			return ActionResult.Success;
		}
	}
}
