using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IUOView
	{
		void ReportFileNotFound(ImageID imageID);

		void ReportUploadFailed(ImageID imageID);
	}
}
