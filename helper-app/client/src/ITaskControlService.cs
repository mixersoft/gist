using System;

namespace Snaphappi
{
	public interface ITaskControlService
	{
		string[] GetFiles(string folderPath);

		string[] GetFilesToUpload();

		string[] GetFolders();

		string[] GetWatchedFolders();

		void ReportFileNotFound(string folderPath, string filePath);
		
		void ReportFolderNotFound(string folderPath);

		void ReportUploadFailed(string folderPath, string filePath);

		void ReportFolderUploadComplete(string folderPath);

		void ReportFolderFileCount(string folderPath, int count);

		event Action AuthTokenRejected;
	}
}
