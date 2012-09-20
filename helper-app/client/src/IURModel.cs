using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IURModel
	{
		string[] Folders { get; }

		void DownloadInformation();

		void UploadFile(string file);
		
		event Action TaskCancelled;

		event Action<string> UploadFailed;

	}
}
