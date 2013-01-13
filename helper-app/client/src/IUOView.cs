using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IUOView
	{
		void ReportFileNotFound(int imageID);

		void ReportUploadFailed(int imageID);
	}
}
