using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IRegistry
	{
		string GetData(string key, string value);

		void SetData(string key, string value, string data);
	}
}
