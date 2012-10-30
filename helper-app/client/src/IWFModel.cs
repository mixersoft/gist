using System;

namespace Snaphappi
{
	public interface IWFModel
	{
		void FetchFiles(string folderPath);

		void FetchFolders();

		void ScheduleFolderUploadCompletionEvent(string folderPath);

		void UploadFile(string folderPath, string filePath);

		void UnscheduleWatcher();

		event Action FolderListEmpty;

		event Action<string> FolderUploadComplete;

		event Action<string> FolderAdded;
	}
}
