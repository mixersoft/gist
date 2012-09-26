using System;

namespace Snaphappi
{
	public class URView : IURView
	{
		#region data

		private readonly IURTaskControlService taskControl;

		#endregion

		#region interface

		public URView(IURTaskControlService taskControl)
		{
			this.taskControl = taskControl;
		}

		#endregion

		#region IURView Members

		public void ReportFolderNotFound(string folder)
		{
			taskControl.ReportFolderNotFound(folder);
		}

		public void ReportUploadFailed(string folder, string path)
		{
			taskControl.ReportUploadFailed(folder, path);
		}

		#endregion
	}
}
