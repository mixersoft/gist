using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IUploadResampledModel
	{
		string[] Folders { get; }

		event Action TaskCancelled;
		event Action InfoDownloaded;

		event Action<string> UploadFailed;

		void DownloadInformation();

		void UploadFile(string file);

	}
}
