using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IURModel
	{
		void DownloadInformation();

		void UploadFile(string folderPath, string filePath);

		event Action<string> FolderAdded;
		
		event Action TaskCancelled;

		event Action<string, string> UploadFailed;

	}
}
