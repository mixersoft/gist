using System;

namespace Snaphappi
{
	public class Registry : IRegistry
	{
		public string GetData(string key, string value)
		{
			return (string)Microsoft.Win32.Registry.GetValue(key, value, null);
		}

		public void SetData(string key, string value, string data)
		{
			Microsoft.Win32.Registry.SetValue(key, value, data);
		}
	}
}
