using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IURModel
	{
		void FetchFiles(string folderPath);

		void FetchFolders();

		int GetFileCount(string folderPath);

		void UploadFile(string folderPath, string filePath);

		event Action<string> FolderAdded;
		
		event Action TaskCancelled;

		event Action<string, string> UploadFailed;

	}
}
