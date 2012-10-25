using System;
using System.Collections.Generic;
using System.Linq;

namespace Snaphappi
{
	public class Multimap<Key, Value>
	{
		private Dictionary<Key, List<Value>> map
			= new Dictionary<Key, List<Value>>();

		public void Add(Key key, Value value)
		{
			if (map.ContainsKey(key))
				map[key].Add(value);
			else
				map.Add(key, new List<Value> { value });
		}

		public IEnumerable<Value> Get(Key key)
		{
			if (map.ContainsKey(key))
				return map[key];
			return Enumerable.Empty<Value>();
		}
	}
}
