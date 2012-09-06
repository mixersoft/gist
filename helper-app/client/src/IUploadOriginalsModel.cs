using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IUploadOriginalsModel
	{
		OriginalFileInfo[] FileInfo { get; }
		string[]           Folders  { get; }

		event Action InfoDownloaded;
		event Action FolderAdded;
		event Action TaskCancelled;

		void DownloadInformation();
	}
}
