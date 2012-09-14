using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IUploadResampledModel
	{
		OriginalFileInfo[] FileInfo { get; }

		event Action TaskCancelled;
		event Action InfoDownloaded;

		event Action<OriginalFileInfo> UploadFailed;

		void DownloadInformation();

		void UploadFile(OriginalFileInfo file);

	}
}
