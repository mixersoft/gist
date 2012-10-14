using Snaphappi;
using System;
using System.Collections.Generic;

namespace SnaphappiTest.src
{
	public class MockRegistry : IRegistry
	{
		private class Entry
		{
			public string Key;
			public string Value;
			public string Data;

			public Entry(string key, string value, string data)
			{
				Key   = key;
				Value = value;
				Data  = data;
			}
		}

		private List<Entry> content = new List<Entry>();

		public string GetData(string key, string value)
		{
			var entry = GetEntry(key, value);
			if (entry == null)
				return null;
			return entry.Data;
		}

		public void SetData(string key, string value, string data)
		{
			var entry = GetEntry(key, value);
			if (entry == null)
			{
				entry = new Entry(key, value, data);
				content.Add(entry);
			}
			entry.Data = data;
		}

		private Entry GetEntry(string key, string value)
		{
			return content.Find(entry => entry.Key == key && entry.Value == value);
		}
	}
}
