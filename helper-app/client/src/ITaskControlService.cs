using System;

namespace Snaphappi
{
	public interface ITaskControlService
	{
		string[] GetFiles(string folderPath);

		UploadTarget[] GetFilesToUpload();

		string[] GetFolders();

		int GetImageHash(ImageID imageID);

		string[] GetWatchedFolders();

		void ReportFileNotFound(string folderPath, string filePath);

		void ReportFileNotFoundByID(ImageID imageID);
		
		void ReportFolderNotFound(string folderPath);

		void ReportUploadFailed(string folderPath, string filePath);

		void ReportUploadFailedByID(ImageID imageID);

		void ReportFolderUploadComplete(string folderPath);

		void ReportFolderFileCount(string folderPath, int count);

		event Action AuthTokenRejected;
	}
}
