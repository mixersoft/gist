using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IUOModel
	{
		OriginalFileInfo[] FileInfo { get; }
		FolderMoveInfo[]   Folders  { get; }

		event Action InfoDownloaded;
		event Action TaskCancelled;

		event Action<OriginalFileInfo> UploadFailed;

		void DownloadInformation();

		void UploadFile(OriginalFileInfo file);
	}
}
