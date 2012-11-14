using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IURView
	{
		void ReportFileCount(string folderPath, int count);

		void ReportFolderNotFound(string folderPath);

		void ReportFolderUploadComplete(string folderPath);

		void ReportUploadFailed(string folderPath, string filePath);
	}
}
