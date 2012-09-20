using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IUOView
	{
		void ReportFolderNotFound(string folder);

		void ReportFileNotFound(OriginalFileInfo file);

		void ReportUploadFailed(OriginalFileInfo file);
	}
}
