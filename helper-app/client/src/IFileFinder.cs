using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IFileFinder
	{
		OriginalFileInfo[] FileInfo { set; }

		void Start();

		void Stop();
		
		event Action<OriginalFileInfo> FileFound;
		event Action<OriginalFileInfo> FileNotFound;
	}
}
