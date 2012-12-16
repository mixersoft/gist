using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IUOView
	{
		void ReportFileNotFound(string folderPath, string filePath);

		void ReportUploadFailed(string folderPath, string filePath);
	}
}
