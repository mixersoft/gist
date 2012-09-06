using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IOriginalFileManager
	{
		void AddFileInfo(OriginalFileInfo[] originalFileInfo);

		OriginalFileInfo[] FileInfo { get; set; }

		string[] Folders { get; set; }

		void Start();

		void Stop();

		event Action<OriginalFileInfo> FileNotFound;
		event Action<string>           FolderNotFound;
		event Action<OriginalFileInfo> UploadFailed;
	}
}
