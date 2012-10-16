using System;

namespace Snaphappi
{
	public class DeviceID
	{
		private readonly IRegistry registry;

		private readonly string key;

		private const string value = @"Device ID";

		public DeviceID(IRegistry registry, string key)
		{
			this.registry = registry;
			this.key      = key;
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
