using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IAppModel
	{
		event Action LoadUploadOriginals;

		void Quit();
	}
}
