using System;

namespace Snaphappi
{
	public interface IWFView
	{
		void ReportFolderNotFound(string folderPath);

		void ReportUploadFailed(string folderPath, string filePath);
	}
}
