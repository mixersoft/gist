using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IURView
	{
		void ReportFolderNotFound(string folderPath);

		void ReportUploadFailed(string folderPath, string filePath);

		void ReportFileCount(string folderPath, int count);

		void ReportFolderUploadComplete(string folderPath);
	}
}
