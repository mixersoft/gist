using System;

namespace Snaphappi
{
	public class UserID
	{
		private readonly IRegistry registry;

		private const string key   = @"HKCU\Software\Snaphappi";
		private const string value = @"User ID";

		public UserID(IRegistry registry)
		{
			this.registry = registry;
		}

		public string GetID()
		{
			string data = registry.GetData(key, value);
			if (data == null)
			{
				data = Guid.NewGuid().ToString();
				registry.SetData(key, value, data);
			}
			return data;
		}
	}
}
