using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IApp
	{
		event Action LoadUploadOriginals;

		void Quit();
	}
}
