using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IApp
	{
		event Action Loaded;
		event Action Terminated;

		void Quit();
	}
}
