using System;

namespace Snaphappi
{
	public class URView : IURView
	{
		#region IURView Members

		public void ReportFolderNotFound(string folder)
		{
			throw new NotImplementedException();
		}

		public void ReportUploadFailed(string path)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
