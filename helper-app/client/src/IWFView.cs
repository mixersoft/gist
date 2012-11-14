using System;

namespace Snaphappi
{
	public interface IWFView
	{
		void ReportFileCount(string folderPath, int count);

		void ReportFolderNotFound(string folderPath);

		void ReportFolderUploadComplete(string folderPath);

		void ReportUploadFailed(string folderPath, string filePath);
	}
}
