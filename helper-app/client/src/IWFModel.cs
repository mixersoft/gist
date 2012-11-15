using System;

namespace Snaphappi
{
	public interface IWFModel
	{
		void FetchFiles(string folderPath);

		void FetchFolders();

		int GetFileCount(string folderPath);

		void ScheduleFolderUploadCompletionEvent(string folderPath);

		void UploadFile(string folderPath, string filePath);

		void UnscheduleWatcher();
		
		event Action                 AuthTokenRejected;
		event Action<string, string> DuplicateUpload;
		event Action<string>         FolderAdded;
		event Action                 FolderListEmpty;
		event Action<string>         FolderUploadComplete;
		event Action<string, string> UploadFailed;
	}
}
