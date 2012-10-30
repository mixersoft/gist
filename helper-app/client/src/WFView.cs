using System;

namespace Snaphappi
{
	public class WFView : IWFView
	{
		#region data

		private readonly IURTaskControlService controlService;

		#endregion // data

		#region interface

		public WFView(IURTaskControlService controlService)
		{
			this.controlService = controlService;
		}

		#endregion // interface

		#region IWFView Members

		public void ReportFolderNotFound(string folderPath)
		{
			controlService.ReportFolderNotFound(folderPath);
		}

		public void ReportUploadFailed(string folderPath, string filePath)
		{
			controlService.ReportUploadFailed(folderPath, filePath);
		}

		#endregion // IWFView Members
	}
}
