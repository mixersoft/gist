using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	interface IURView
	{
		void ReportFolderNotFound(string folder);

		void ReportUploadFailed(string folder, string path);

		void ReportFileCount(string folderPath, int count);
	}
}
